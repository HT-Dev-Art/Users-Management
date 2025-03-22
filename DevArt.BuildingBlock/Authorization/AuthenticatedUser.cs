using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using PermissionsStatic = DevArt.BuildingBlock.Permissions;

namespace DevArt.BuildingBlock.Authorization;

public record AuthenticatedUser
{
    public AuthenticatedUser(HttpContext context, string authority)
    {
        Id = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
        Permissions = context.User.FindAll(c => c.Type == PermissionsStatic.ClaimType
                                                && c.Issuer == authority)
            .Aggregate(PermissionsFlags.None, (permissionsEnums, claim) =>
            {
                if (!PermissionsStatic.PermissionssDictionary.TryGetValue(claim.Value, out var permissionEnum))
                {
                    return permissionsEnums;
                }
                permissionsEnums |= permissionEnum;
                return permissionsEnums;
            });
        IsValid = !string.IsNullOrWhiteSpace(Id) && Permissions != PermissionsFlags.None;
    }

    public PermissionsFlags Permissions { get; }

    public string Id { get; }

    public bool IsValid { get; }

    public bool IsHavePermission(string auth0Id)
    {
        return Id == auth0Id;
    }
}