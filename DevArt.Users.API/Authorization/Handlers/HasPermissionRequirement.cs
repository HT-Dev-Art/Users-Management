using DevArt.BuildingBlock;
using Microsoft.AspNetCore.Authorization;

namespace DevArt.Users.API.Authorization.Handlers;

public class HasPermissionRequirement(string issuer, PermissionsFlags permissions) : IAuthorizationRequirement
{
    public string Issuer { get; } = issuer ?? throw new ArgumentNullException(nameof(issuer));

    public PermissionsFlags Permissions { get; } = permissions;
}