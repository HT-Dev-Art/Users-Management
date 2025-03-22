using System.Security.Claims;
using DevArt.BuildingBlock.Authorization;
using DevArt.Users.API.Authorization;
using DevArt.Users.API.Authorization.Handlers;
using DevArt.Users.API.Validation;
using DevArt.Users.Application.Configuration;
using DevArt.Users.Application.Service;
using DevArt.Users.Application.Service.Impl;
using DevArt.Users.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace DevArt.Users.API.Extensions;

public static class Extension
{
    public static void AddApplicationService(this IHostApplicationBuilder builder)
    {
        var serviceCollection = builder.Services;
        serviceCollection.AddDbContext<UserContext>(option =>
            option.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        var auth0Config = builder.Configuration.GetSection("Auth0Config");
        serviceCollection.Configure<Auth0Config>(config: auth0Config);
        serviceCollection.Configure<JwtBearerOptions>(
            option =>
        {
            option.Authority = builder.Configuration["JwtSetting:Authority"];
            option.Audience = builder.Configuration["JwtSetting:Audience"];
            option.TokenValidationParameters = new TokenValidationParameters
            {
                ClockSkew = TimeSpan.FromMinutes(5),
                ValidIssuer = builder.Configuration["JwtSetting:Authority"],
                ValidAudiences = [builder.Configuration["JwtSetting:Audience"],  builder.Configuration["JwtSetting:Auth0Audience"]]
            };
        });
        
        serviceCollection.AddHttpContextAccessor();
        serviceCollection.AddControllers();
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddValidatorsFromAssemblyContaining<CreateUserValidation>();
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(option =>
            {
                option.Authority = builder.Configuration["JwtSetting:Authority"];
                option.Audience = builder.Configuration["JwtSetting:Audience"];
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    ClockSkew = TimeSpan.FromMinutes(5),
                    ValidIssuer = builder.Configuration["JwtSetting:Authority"],
                    ValidAudiences = [builder.Configuration["JwtSetting:Audience"],  builder.Configuration["JwtSetting:Auth0Audience"]]
                };
            });
        
        serviceCollection.AddAuthorization();
        serviceCollection.AddSingleton<IAuth0Service, Auth0Service>();
        serviceCollection.AddSingleton<IAuthorizationPolicyProvider, HasPermissionPolicyProvider>();
        serviceCollection.AddSingleton<IAuth0Service, Auth0Service>();
        serviceCollection.AddSingleton<IAuthorizationPolicyProvider, HasPermissionPolicyProvider>();
        serviceCollection.AddScoped<IUserService, UserService>();
        serviceCollection.AddScoped<IAuthenticatedUserProvider, AuthenticatedUserProvider>();
        serviceCollection.AddScoped<IAuthorizationHandler, HasPermissionHandler>();
        serviceCollection.AddScoped<IUserService, UserService>();
        serviceCollection.AddScoped<IAuthenticatedUserProvider, AuthenticatedUserProvider>();
        serviceCollection.AddScoped<IAuthorizationHandler, HasPermissionHandler>();
    }

    public static void RunMigration(this WebApplication webApplication)
    {
        using var scope = webApplication.Services.CreateScope();
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<UserContext>();
        dbContext.Database.Migrate();
    }
}