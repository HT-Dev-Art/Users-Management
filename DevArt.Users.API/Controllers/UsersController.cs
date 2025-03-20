using DevArt.Users.Application.Dto;
using DevArt.Users.Application.Service;
using Microsoft.AspNetCore.Mvc;

namespace DevArt.Users.API.Controllers;

public class UsersController(IUserService userService) : BaseController
{
    [HttpGet("{id:int}")]
    public IActionResult GetUserById(int id)
    {
        var userResult = userService.GetUserById(id);
        return userResult.ConvertTo<IActionResult>(Ok, 
            exception => BadRequest(userResult.Exception!.Message));
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        var userResult = await userService.CreateUser(createUserDto);

        return userResult.ConvertTo<IActionResult>(success => Created("",success), 
            exception => BadRequest(exception.Message));
    }

    [HttpPatch("{id:int}")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto, int id)
    {
        var userResult = await userService.UpdateUser(updateUserDto, id);

        return userResult.ConvertTo<IActionResult>(Ok,
            exception => BadRequest(exception.Message));
    }
}