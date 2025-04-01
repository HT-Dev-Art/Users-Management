using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using DevArt.Core.Results;
using DevArt.Users.Application.Configuration;
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
            { { "password", updateUserDto.NewPassword }, { "nickname", updateUserDto.NickName } };

        foreach (var key in bodyDictionary.Keys.Where(key =>
                     bodyDictionary[key] == null || bodyDictionary[key] == string.Empty)) bodyDictionary.Remove(key);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        var bodySerialize = JsonSerializer.Serialize(bodyDictionary, jsonOptions);
        var client = await GetAuth0Client();
        var response = await client.PatchAsync($"users/{auth0Id}",
            new StringContent(bodySerialize, Encoding.UTF8, "application/json"));
        if (!response.IsSuccessStatusCode)
            return new FailedUpdateUserException("Cannot update your account. Please try again!");
        var contentStream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<Auth0ResponseDto>(contentStream);
        return result ?? new Auth0ResponseDto();
    }


    private async Task<HttpClient> GetAuth0Client()
    {
        var token = await GetToken();
        var client = httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        client.BaseAddress = new Uri(_auth0Config.ManagementEndPoint);
        return client;
    }


    private async Task<string> RefreshToken()
    {
        var client = httpClientFactory.CreateClient();

        client.BaseAddress = new Uri(_auth0Config.OAuthTokenUrl);
        var body = new Dictionary<string, string>
        {
            { "audience", _auth0Config.ManagementEndPoint },
            { "client_id", _auth0Config.ClientId },
            { "client_secret", _auth0Config.ClientSecret },
            { "grant_type", _auth0Config.GrantType }
        };
        var bodySerialize = JsonSerializer.Serialize(body);
        var response = await client.PostAsync(string.Empty, new StringContent(bodySerialize,
            Encoding.UTF8, "application/json"));
        var contentStream = await response.Content.ReadAsStreamAsync();
        var auth0Credential = JsonSerializer.DeserializeAsync<Auth0CredentialDto>(contentStream);
        return auth0Credential.Result?.AccessToken ?? "";
    }

    private async Task<string> GetToken()
    {
        memoryCache.TryGetValue(_auth0TokenKey, out string? token);
        if (token is not null) return token;
        token = await RefreshToken();
        memoryCache.Set(_auth0TokenKey, token, TimeSpan.FromDays(1));
        return token;
    }
}