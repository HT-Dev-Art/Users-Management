using DevArt.Core.Results;
using DevArt.Users.Application.Dto;

namespace DevArt.Users.Application.Service;

public interface IUserService
{
    Task<Result<bool>> UpdateUser(UpdateUserDto updateUserDto);

    Task<UserDto> GetOrCreateMappingUser();
}