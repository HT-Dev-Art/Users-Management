using DevArt.BuildingBlock.Authorization;
using DevArt.Users.Application.Configuration;
using DevArt.Users.Application.Constants;
using DevArt.Users.Application.Dto;
using DevArt.Users.Application.Exceptions;
using DevArt.Users.Application.Mapper;
using DevArt.Users.Core;
using DevArt.Users.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DevArt.Users.Application.Service.Impl;

public class UserService(UserContext userContext, 
    IAuth0Service auth0Service, 
    IAuthenticatedUserProvider authenticatedUserProvider,
    IOptions<Auth0Config> auth0ConfigOption,
    ILogger<UserService> logger) : IUserService
{
    private Auth0Config _auth0Config = auth0ConfigOption.Value;
    public Result<UserDto> GetUserById(int id)
    {
        logger.LogInformation("Get user by {Id}", id);
        var selectedUser = userContext.Users.Find(id);

        if (selectedUser is null)
        {
            logger.LogInformation("Cannot found userId {Id}", id);
            return new UserNotFoundException("Requested user is not found");
        } 
        
        var performUser = authenticatedUserProvider.User;

        if (!performUser.IsHavePermission(selectedUser.Auth0Id))
        {
            logger.LogInformation("User with Auth0Id {Auth0Id} try to get information of userId {Id}", performUser.Id, id);
            return new UserBrokenAccessException("You don't have permission to access");   
        }

        var userDto = UserMapper.ToUserDto(selectedUser);

        return userDto;

    }

    public async Task<Result<UserDto>> CreateUser(CreateUserDto createUserDto)
    {
        var isExistingEmail = userContext.Users
            .Where(user => user.Email == createUserDto.Email)
            .Select(user => user.Email)
            .FirstOrDefault();
        if (isExistingEmail is not null)
        {
            return new UserExistingException("This email has been used. Please use another one");
        } 
        logger.LogInformation("Creating user {@CreateUserDto}", createUserDto);
        var userResponse = await auth0Service.CreateUser(createUserDto);
        logger.LogInformation("Auth0 response {@Auth0Response}", userResponse);
        var userRole = _auth0Config.Roles.
            First(role => role.Key == Auth0Constant.userRole);
        var roleResponse = await auth0Service.UpdateUserRole(userRole.Value, [userResponse.UserId]);
        if (!roleResponse.IsSuccessful)
        {
            logger.LogError("Failed assign role to user with Auth0Id {@Id}", userResponse.UserId);
            logger.LogInformation("Deleting user with Auth0Id {@Id}", userResponse.UserId);
            await auth0Service.DeleteUser(userResponse.UserId);
            return new CannotUnloadAppDomainException("Failed to created your account. Please try again");
        }
        var newUser = UserMapper.ToUser(userResponse);
        await userContext.AddAsync(newUser);
        await userContext.SaveChangesAsync();
        var userDto = UserMapper.ToUserDto(newUser);
        return userDto;
    }
    

    public async Task<Result<UserDto>> UpdateUser(UpdateUserDto updateUserDto, int id)
    {
        var selectedUser = await userContext.Users.FindAsync(id);
        
        if (selectedUser is null)  return new UserNotFoundException("Requested user is not found");
        
        var performUser = authenticatedUserProvider.User;

        if (!performUser.IsHavePermission(selectedUser.Auth0Id))
        {
            logger.LogInformation("Cannot update user with Id {Id} due to lacking off permission", id);
            return new UserBrokenAccessException("You don't have permission to perform update user");
        }

        var response = await auth0Service.UpdateUser(updateUserDto, selectedUser.Auth0Id);
        
        selectedUser.UpdateUser(response);

        await userContext.SaveChangesAsync();

        var userDto = UserMapper.ToUserDto(selectedUser);

        return userDto;

    }


    
}