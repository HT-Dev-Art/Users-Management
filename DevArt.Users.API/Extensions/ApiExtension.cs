using System.Net.Http.Headers;
using DevArt.Users.Application.Configuration;
using DevArt.Users.Application.Constants;
using Microsoft.Extensions.Options;

namespace DevArt.Users.API.Extensions;

public static class ApiExtension
{
    public static void AddThirdPartiesClient(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddHttpClient(ApplicationConstants.Auth0ClientName, (provider, client) =>
        {
            var auth0Config = provider
                .GetRequiredService<IOptions<Auth0Config>>().Value;
            client.BaseAddress = new Uri(auth0Config.Auth0Domain);
            client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json; charset=utf-8");
        });
    }
}