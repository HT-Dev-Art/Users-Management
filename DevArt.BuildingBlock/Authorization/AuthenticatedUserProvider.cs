using DevArt.BuildingBlock.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace DevArt.BuildingBlock.Authorization;

public class AuthenticatedUserProvider : IAuthenticatedUserProvider
{
    public AuthenticatedUserProvider(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        var context = httpContextAccessor.HttpContext
                      ?? throw new HttpContextNotFoundException("HttpContext not found.");

        User = new AuthenticatedUser(context, configuration["JwtSetting:Authority"] ?? string.Empty);
    }
    
    public AuthenticatedUser User { get; }
}