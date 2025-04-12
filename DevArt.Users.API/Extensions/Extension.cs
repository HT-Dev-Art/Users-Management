using DevArt.API.Authorization;
using DevArt.Users.API.Authorization;
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
        
        serviceCollection.Configure<Auth0Config>(config: builder.Configuration.GetSection("Auth0Config"));
        serviceCollection.Configure<JwtBearerOptions>(config: builder.Configuration.GetSection("JwtSetting"));
        
        serviceCollection.AddControllers();
        serviceCollection.AddHttpClient();
        serviceCollection.AddMemoryCache();
        builder.Services.AddAuthorizationHandler();
        builder.Services.AddProblemDetails();
        builder.Services.AddThirdPartiesClient();
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddValidatorsFromAssemblyContaining<UpdateUserValidation>();
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
                    ValidAudiences = builder.Configuration
                        .GetSection("JwtSetting:TokenValidationParameters:ValidAudiences")
                        .Get<List<string>>()
                };
            });

        serviceCollection.AddAuthorization();
        serviceCollection.AddSingleton<IAuth0Service, Auth0Service>();
        serviceCollection.AddSingleton<IAuthorizationPolicyProvider, HasScopePolicyProvider>();
        serviceCollection.AddScoped<IUserService, UserService>();
    }
}
