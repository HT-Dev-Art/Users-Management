using DevArt.Users.Application.Dto;
using DevArt.Users.Application.Dto.Auth0;

namespace DevArt.Users.Application.Service;

public interface IAuth0Service
{
    Task<Auth0ResponseDto> CreateUser(CreateUserDto userDto);

    Task<Auth0ResponseDto> UpdateUser(UpdateUserDto updateUserDto, string auth0Id);
}