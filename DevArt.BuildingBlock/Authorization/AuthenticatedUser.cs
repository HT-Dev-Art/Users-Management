using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ScopesStaticClass = DevArt.BuildingBlock.Scopes;

namespace DevArt.BuildingBlock.Authorization;

public record AuthenticatedUser
{
    public AuthenticatedUser(HttpContext context, string authority)
    {
        Id = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        Scopes = context.User
                     .FindFirst(c => c.Type == ScopesStaticClass.ClaimType && c.Issuer == authority)
                     ?.Value
                     .Split(' ')
                     .Aggregate(
                         ScopesFlags.None,
                         (scopesEnum, scope) =>
                         {
                             if (!ScopesStaticClass.ScopesDictionary.TryGetValue(scope, out var scopeEnum))
                             {
                                 return scopesEnum;
                             }

                             scopesEnum |= scopeEnum;

                             return scopesEnum;
                         })
                 ?? ScopesFlags.None;
        IsValid = !string.IsNullOrWhiteSpace(Id) && Scopes != ScopesFlags.None;
    }

    public ScopesFlags Scopes { get; }

    public string Id { get; }

    public bool IsValid { get; }

    public bool IsHavePermission(string auth0Id)
    {
        return Id == auth0Id;
    }
}