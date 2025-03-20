using DevArt.BuildingBlock.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace DevArt.Users.API.Authorization.Handlers;

public sealed class HasScopeHandler(IAuthenticatedUserProvider authenticatedUserProvider)
    : AuthorizationHandler<HasScopeRequirement>
{

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasScopeRequirement requirement)
    {
        if (!authenticatedUserProvider.User.IsValid)
        {
            return Task.CompletedTask;
        }

        if ((authenticatedUserProvider.User.Scopes & requirement.Scopes) == requirement.Scopes)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}