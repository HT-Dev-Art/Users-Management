using DevArt.BuildingBlock.Authorization;
using DevArt.Users.Application.Dto;
using DevArt.Users.Application.Exceptions;
using DevArt.Users.Application.Mapper;
using DevArt.Users.Core;
using DevArt.Users.Infrastructure;
using Microsoft.Extensions.Logging;

namespace DevArt.Users.Application.Service.Impl;

public class UserService(UserContext userContext, 
    IAuth0Service auth0Service, 
    IAuthenticatedUserProvider authenticatedUserProvider,
    ILogger<UserService> logger) : IUserService
{
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

        if (performUser.IsHavePermission(selectedUser.Auth0Id))
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
        if (isExistingEmail is null)
        {
            return new UserExistingException("This email has been used. Please use another one");
        } 
        logger.LogInformation("Creating user @{CreateUserDto}", createUserDto);
        var response = await auth0Service.CreateUser(createUserDto);
        logger.LogInformation("Auth0 response @{Auth0Response}", response);
        var newUser = UserMapper.ToUser(response);
        await userContext.AddAsync(newUser);
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
            return new UserBrokenAccessException("You don't have permission to perform update user");
        }

        var response = await auth0Service.UpdateUser(updateUserDto, selectedUser.Auth0Id);
        
        selectedUser.UpdateUser(response);

        await userContext.SaveChangesAsync();

        var userDto = UserMapper.ToUserDto(selectedUser);

        return userDto;

    }


    
}