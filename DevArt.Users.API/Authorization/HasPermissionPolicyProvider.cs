using DevArt.BuildingBlock;
using DevArt.Users.API.Authorization.Handlers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace DevArt.Users.API.Authorization;

public class HasPermissionPolicyProvider(IOptions<AuthorizationOptions> options,IConfiguration configuration) : IAuthorizationPolicyProvider

{
    private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider = new(options);

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!Permissions.PermissionssDictionary.TryGetValue(policyName, out var scopeFlag))
        {
            return  Task.FromResult<AuthorizationPolicy?>(null);
        }
        
        var policy = new AuthorizationPolicyBuilder();
        policy.Requirements.Add(new HasPermissionRequirement(configuration["JwtSetting:Authority"] ?? "", scopeFlag));
        return Task.FromResult(policy.Build())!;
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
        => _fallbackPolicyProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
        => _fallbackPolicyProvider.GetFallbackPolicyAsync();
}