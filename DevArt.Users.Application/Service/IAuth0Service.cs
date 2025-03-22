using DevArt.Users.Application.Dto;
using DevArt.Users.Application.Dto.Auth0;
using RestSharp;

namespace DevArt.Users.Application.Service;

public interface IAuth0Service
{
    Task<Auth0ResponseDto> CreateUser(CreateUserDto userDto);

    Task<Auth0ResponseDto> UpdateUser(UpdateUserDto updateUserDto, string auth0Id);

    Task<RestResponse> UpdateUserRole(string roleId, List<string> auth0Ids);

    Task<RestResponse> DeleteUser(string auth0Id);
}