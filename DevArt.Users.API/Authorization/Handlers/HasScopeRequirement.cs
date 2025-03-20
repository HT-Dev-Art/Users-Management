using DevArt.BuildingBlock;
using Microsoft.AspNetCore.Authorization;

namespace DevArt.Users.API.Authorization.Handlers;

public class HasScopeRequirement(string issuer, ScopesFlags scopes) : IAuthorizationRequirement
{
    public string Issuer { get; } = issuer ?? throw new ArgumentNullException(nameof(issuer));

    public ScopesFlags Scopes { get; } = scopes;
}