using DevArt.Users.Application.Dto;
using DevArt.Users.Core;
using DevArt.Users.Infrastructure;

namespace DevArt.Users.Application.Service;

public interface IUserService
{
    Result<UserDto> GetUserById(int id);

    Task<Result<UserDto>> CreateUser(CreateUserDto createUserDto);

    Task<Result<UserDto>> UpdateUser(UpdateUserDto updateUserDto, int id);
}