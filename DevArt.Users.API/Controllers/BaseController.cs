using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevArt.Users.API.Controllers;

[Authorize]
[Route("/api/[controller]")]
public class BaseController : ControllerBase
{
    
}