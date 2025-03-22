using DevArt.BuildingBlock.Authorization;
using Microsoft.AspNetCore.Authorization;

namespace DevArt.Users.API.Authorization.Handlers;

public sealed class HasPermissionHandler(IAuthenticatedUserProvider authenticatedUserProvider)
    : AuthorizationHandler<HasPermissionRequirement>
{

    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPermissionRequirement requirement)
    {
        if (!authenticatedUserProvider.User.IsValid)
        {
            return Task.CompletedTask;
        }

        if ((authenticatedUserProvider.User.Permissions & requirement.Permissions) == requirement.Permissions)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}