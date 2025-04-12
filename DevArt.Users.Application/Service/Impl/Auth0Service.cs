using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using DevArt.Core.Results;
using DevArt.Users.Application.Configuration;
using DevArt.Users.Application.Constants;
using DevArt.Users.Application.Dto;
using DevArt.Users.Application.Dto.Auth0;
using DevArt.Users.Application.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace DevArt.Users.Application.Service.Impl;

public class Auth0Service(
    IHttpClientFactory httpClientFactory,
    IMemoryCache memoryCache,
    IOptions<Auth0Config> auth0ConfigSnapshot) : IAuth0Service
{
    private readonly Auth0Config _auth0Config = auth0ConfigSnapshot.Value;
    private readonly string _auth0TokenKey = "token";

    public async Task<Result<Auth0ResponseDto>> UpdateUser(UpdateUserDto updateUserDto, string auth0Id)
    {
        var bodyDictionary = new Dictionary<string, string?>
        {
            { "nickname", updateUserDto.NickName }
        };

        foreach (var key in bodyDictionary.Keys.Where(key =>
                     bodyDictionary[key] is null ||
                     bodyDictionary[key] == string.Empty))
        {
            bodyDictionary.Remove(key);
        }
        

        var httpRequestOption = new HttpRequestMessageOptionDto<Dictionary<string, string?>>()
        {
            Route = $"{ApplicationConstants.Auth0ManagementRoute}users/{auth0Id}",
            Body = bodyDictionary,
            AttachAuthorizationHeader = true,
            Method = HttpMethod.Patch
        };
        var response = await HandleResponse<Dictionary<string, string?>, Auth0ResponseDto>(httpRequestOption);
        return response;
    }

    private async Task<string> RefreshToken()
    {
        var body = new Dictionary<string, string>
        {
            { "audience", $"{_auth0Config.Auth0Domain}{ApplicationConstants.Auth0ManagementRoute}" },
            { "client_id", _auth0Config.ClientId },
            { "client_secret", _auth0Config.ClientSecret },
            { "grant_type", _auth0Config.GrantType }
        };

        var httpRequestOption = new HttpRequestMessageOptionDto<Dictionary<string, string>>()
        {
            Route = ApplicationConstants.Auth0OAuthRoute,
            Body = body,
            AttachAuthorizationHeader = false,
            Method = HttpMethod.Post
        };

        var result = await HandleResponse<Dictionary<string, string>, Auth0CredentialDto>(
            httpRequestOption);

        return result.Value?.AccessToken ?? "";
    }

    private async Task<string> GetToken()
    {
        memoryCache.TryGetValue(_auth0TokenKey, out string? token);
        if (token is not null && token != string.Empty) return token;
        token = await RefreshToken();
        memoryCache.Set(_auth0TokenKey, token,
            TimeSpan.FromDays(ApplicationConstants.ExpirationDate));
        return token;
    }

    private async Task<Result<TResponse>> HandleResponse<TBody, TResponse>(
        HttpRequestMessageOptionDto<TBody> httpRequestMessageOptionDto)
    {
        var client = httpClientFactory.CreateClient(ApplicationConstants.Auth0ClientName);
        var jsonOption = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        var httpRequestMessage = new HttpRequestMessage
        {
            Content = JsonContent.Create(httpRequestMessageOptionDto.Body),
            Method = httpRequestMessageOptionDto.Method,
            RequestUri = new Uri(client.BaseAddress ?? new Uri(_auth0Config.Auth0Domain),
                httpRequestMessageOptionDto.Route)
        };
        if (httpRequestMessageOptionDto.AttachAuthorizationHeader)
        {
            var token = await GetToken();
            httpRequestMessage.Headers.Authorization =
                new AuthenticationHeaderValue(ApplicationConstants.AuthenticationSchema, token);
        }

        var responseMessage = await client.SendAsync(httpRequestMessage);
        if (!responseMessage.IsSuccessStatusCode) return new BadHttpClientException("Failed to interact resource");
        var contentStream = await responseMessage.Content.ReadAsStreamAsync();
        var deserializeContent = await JsonSerializer.DeserializeAsync<TResponse>(contentStream, jsonOption);
        if (deserializeContent is null) return new JsonException("Failed deserialization content");
        return deserializeContent;
    }
}
