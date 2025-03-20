using DevArt.Users.Application.Dto;
using DevArt.Users.Application.Dto.Auth0;
using DevArt.Users.Core.Entities;
using Riok.Mapperly.Abstractions;

namespace DevArt.Users.Application.Mapper;

[Mapper]
public static partial  class UserMapper
{
    public static partial UserDto ToUserDto(User user);

    [MapProperty(nameof(Auth0ResponseDto.UserId), nameof(User.Auth0Id))]
    public static partial User ToUser(Auth0ResponseDto auth0ResponseDto);


    public static partial void UpdateUser([MappingTarget] this User user, Auth0ResponseDto auth0ResponseDto);
}