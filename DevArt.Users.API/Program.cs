using DevArt.API.Authorization;
using DevArt.Users.API.Extensions;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}
else
{
    builder.Configuration["Auth0Config:ClientId"] = Environment.GetEnvironmentVariable("Auth0Config:ClientId");
    builder.Configuration["Auth0Config:ClientSecret"] = Environment.GetEnvironmentVariable("Auth0Config:ClientSecret");
    builder.Configuration["Auth0Config:GrantType"] = Environment.GetEnvironmentVariable("Auth0Config:GrantType");
    builder.Configuration["Auth0Config:OAuthTokenUrl"] = Environment.GetEnvironmentVariable("Auth0Config:OAuthTokenUrl");
    builder.Configuration["Auth0Config:ManagementEndPoint"] = Environment.GetEnvironmentVariable("Auth0Config:ManagementEndPoint");
    builder.Configuration["JwtSetting:Authority"] = Environment.GetEnvironmentVariable("JwtSetting:Authority");
    builder.Configuration["JwtSetting:Audience"] = Environment.GetEnvironmentVariable("JwtSetting:Audience");
    builder.Configuration["JwtSetting:Auth0Audience"] = Environment.GetEnvironmentVariable("JwtSetting:Auth0Audience");
}


builder.Services.AddAuthorizationHandler();
builder.Services.AddProblemDetails();
builder.AddApplicationService();
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("OpenApi/v1.json");
    app.MapScalarApiReference();
}

app.MapControllers();

app.UseAuthentication();

app.UseAuthorization();

app.UseHttpsRedirection();

app.RunMigration();

app.Run();
