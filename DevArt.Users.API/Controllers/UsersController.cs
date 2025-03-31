using DevArt.API.Authorization;
using DevArt.API.Controllers;
using DevArt.Users.Application.Dto;
using DevArt.Users.Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevArt.Users.API.Controllers;

public class UsersController(IUserService userService) : SecuredController
{
    
    [Authorize(Policy = Scopes.WriteCurrentUser)]
    [HttpPatch("current-user")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
    {
        var userResult = await userService.UpdateUser(updateUserDto);

        return userResult.ConvertTo<IActionResult>(_ => NoContent(),
            exception => BadRequest(new ProblemDetails(
                )
            {
                Detail = exception.Message
            }));
    }
    

    [Authorize]
    [HttpGet("current-user")]
    public async Task<IActionResult> GetOrMappingUser()
    {
        var result = await userService.GetOrCreateMappingUser();
        return Ok(result);
    }
}
