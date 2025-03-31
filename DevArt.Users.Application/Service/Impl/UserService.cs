using DevArt.API.Authorization;
using DevArt.Core.Results;
using DevArt.Users.Application.Dto;
using DevArt.Users.Application.Exceptions;
using DevArt.Users.Application.Mapper;
using DevArt.Users.Core.Entities;
using DevArt.Users.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace DevArt.Users.Application.Service.Impl;

public class UserService(
    UserContext userContext,
    IAuth0Service auth0Service,
    IAuthenticatedUserProvider authenticatedUserProvider,
    IMemoryCache memoryCache,
    ILogger<UserService> logger) : IUserService
{
    
    public async Task<Result<bool>> UpdateUser(UpdateUserDto updateUserDto)
    {
        var currentAuth0Id = authenticatedUserProvider.User.Id;

        var selectedUser = await userContext.Users
            .FirstOrDefaultAsync(user => user.Auth0Id == currentAuth0Id);
        if (selectedUser is null)
        {
            logger.LogError("Cannot find requested user with auth0Id {Auth0Id}", currentAuth0Id);
            return new UserNotFoundException("Cannot found requested user");
        }
        
        var response = await auth0Service.UpdateUser(updateUserDto, selectedUser.Auth0Id);

        if (response is { IsSuccess: false, Exception: not null }) return response.Exception;   

        return response.IsSuccess;
    }

    public async Task<UserDto> GetOrCreateMappingUser()
    {
        const int notFound = 0;
        User? selectedUser;
        var currentUserAuth0Id = authenticatedUserProvider.User.Id;

        memoryCache.TryGetValue(currentUserAuth0Id, out int? selectedId);

        selectedId ??= await userContext.Users
            .Where(user => user.Auth0Id == currentUserAuth0Id)
            .Select(user => user.Id)
            .FirstOrDefaultAsync();

        
        if (selectedId == notFound)
        {
            selectedUser = new User()
            {
                Auth0Id = currentUserAuth0Id
            };
            await userContext.AddAsync(selectedUser);
            await userContext.SaveChangesAsync();
        }
        else
        {
            selectedUser = await userContext.Users.FindAsync(selectedId);
        }

        
        
        var userDto = UserMapper.ToUserDto(user: selectedUser);
        
        return userDto;
    }
}
