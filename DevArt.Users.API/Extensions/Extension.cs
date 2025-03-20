using System.Security.Claims;
using DevArt.BuildingBlock.Authorization;
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

namespace DevArt.Users.API.Extensions;

public static class Extension
{
    public static void AddApplicationService(this IHostApplicationBuilder builder)
    {
        var serviceCollection = builder.Services;
        serviceCollection.AddDbContext<UserContext>(option =>
            option.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        serviceCollection.Configure<Auth0Config>(config: 
            builder.Configuration.GetSection("Auth0Config"));
        
        serviceCollection.AddHttpContextAccessor();
        serviceCollection.AddControllers();

        serviceCollection.AddSingleton<IAuth0Service, Auth0Service>();
        serviceCollection.AddScoped<IUserService, UserService>();
        serviceCollection.AddScoped<IAuthenticatedUserProvider, AuthenticatedUserProvider>();
        serviceCollection.AddScoped<IAuthorizationHandler, HasScopeHandler>();
        
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(option =>
            {
                option.Authority = builder.Configuration.GetSection("Auth0Config")["TenantUrl"];
                option.Audience = builder.Configuration.GetSection("Auth0Config")["Audience"];
                option.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.NameIdentifier,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };
            });

        builder.Services.AddValidatorsFromAssembly(typeof(CreateUserValidation).Assembly);
    }

    public static void RunMigration(this WebApplication webApplication)
    {
        using var scope = webApplication.Services.CreateScope();
        var services = scope.ServiceProvider;
        var dbContext = services.GetRequiredService<UserContext>();
        dbContext.Database.Migrate();
    }
}