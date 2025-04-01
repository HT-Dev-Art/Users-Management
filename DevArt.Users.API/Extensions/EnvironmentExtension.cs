namespace DevArt.Users.API.Extensions;

public static class EnvironmentExtension
{
    public static void AddEnvironmentValues(this IHostApplicationBuilder builder)
    {
        builder.Configuration["Auth0Config:ClientId"] = Environment.GetEnvironmentVariable("Auth0Config:ClientId");
        builder.Configuration["Auth0Config:ClientSecret"] =
            Environment.GetEnvironmentVariable("Auth0Config:ClientSecret");
        builder.Configuration["Auth0Config:GrantType"] = Environment.GetEnvironmentVariable("Auth0Config:GrantType");
        builder.Configuration["Auth0Config:OAuthTokenUrl"] =
            Environment.GetEnvironmentVariable("Auth0Config:OAuthTokenUrl");
        builder.Configuration["Auth0Config:ManagementEndPoint"] =
            Environment.GetEnvironmentVariable("Auth0Config:ManagementEndPoint");
        builder.Configuration["JwtSetting:Authority"] = Environment.GetEnvironmentVariable("JwtSetting:Authority");
        builder.Configuration["JwtSetting:Audience"] = Environment.GetEnvironmentVariable("JwtSetting:Audience");
        builder.Configuration["JwtSetting:Auth0Audience"] =
            Environment.GetEnvironmentVariable("JwtSetting:Auth0Audience");
    }
}