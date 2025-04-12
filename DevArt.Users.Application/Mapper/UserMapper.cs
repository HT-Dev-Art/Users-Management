using DevArt.Users.Application.Dto;
using DevArt.Users.Application.Dto.Auth0;
using DevArt.Users.Core.Entities;
using Riok.Mapperly.Abstractions;

namespace DevArt.Users.Application.Mapper;

[Mapper]
public static partial  class UserMapper
{
    public static partial UserDto ToUserDto(User? user);
}
