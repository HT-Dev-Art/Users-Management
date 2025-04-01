using DevArt.Core.Results;
using DevArt.Users.Application.Dto;
using DevArt.Users.Application.Dto.Auth0;

namespace DevArt.Users.Application.Service;

public interface IAuth0Service
{
    Task<Result<Auth0ResponseDto>> UpdateUser(UpdateUserDto updateUserDto, string auth0Id);
}