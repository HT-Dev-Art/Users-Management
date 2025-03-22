using System.ComponentModel;
using DevArt.BuildingBlock;
using DevArt.Users.Application.Dto;
using DevArt.Users.Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevArt.Users.API.Controllers;

public class UsersController(IUserService userService) : BaseController
{
    [Authorize(Policy = Permissions.ReadCurrentUser)]
    [HttpGet("{id:int}")]
    public IActionResult GetUserById(int id)
    {
        var userResult = userService.GetUserById(id);
        return userResult.ConvertTo<IActionResult>(Ok, 
            exception => BadRequest(exception.Message));
    }
    
    [AllowAnonymous]
    [EndpointSummary("Create a user")]
    [Description("It allow to create user")]
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