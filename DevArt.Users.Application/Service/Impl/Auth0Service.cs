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
        var bodyDictionary = new Dictionary<string, string>
        {
            { "password", updateUserDto.NewPassword ?? "" },
            { "nickname", updateUserDto.NickName ?? "" }
        };

        foreach (var key in bodyDictionary.Keys.Where(key =>
                     bodyDictionary[key] == string.Empty)) bodyDictionary.Remove(key);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        var client = httpClientFactory.CreateClient(ApplicationConstants.Auth0ClientName);
        var token = await GetToken();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(ApplicationConstants.AuthenticationSchema, token);
        var response = await client.PatchAsJsonAsync($"{ApplicationConstants.Auth0ManagementRoute}/users/{auth0Id}", bodyDictionary);
        if (!response.IsSuccessStatusCode)
            return new FailedUpdateUserException("Cannot update your account. Please try again!");
        var contentStream = await response.Content.ReadAsStreamAsync();
        var result = await JsonSerializer.DeserializeAsync<Auth0ResponseDto>(contentStream, jsonOptions);
        return result ?? new Auth0ResponseDto();
    }


    private async Task<string> RefreshToken()   
    {
        var client = httpClientFactory.CreateClient(ApplicationConstants.Auth0ClientName);

        var body = new Dictionary<string, string>
        {
            { "audience", $"{_auth0Config.Auth0Domain}api/v2/" },
            { "client_id", _auth0Config.ClientId },
            { "client_secret", _auth0Config.ClientSecret },
            { "grant_type", _auth0Config.GrantType }
        };
        var jsonOption = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        };
        var response = await client.PostAsJsonAsync("oauth/token", body);
        var contentStream = await response.Content.ReadAsStreamAsync();
        var auth0Credential = JsonSerializer.DeserializeAsync<Auth0CredentialDto>(contentStream, jsonOption);
        return auth0Credential.Result?.AccessToken ?? "";
    }

    private async Task<string> GetToken()
    {
        memoryCache.TryGetValue(_auth0TokenKey, out string? token);
        if (token is not null) return token;
        token = await RefreshToken();
        memoryCache.Set(_auth0TokenKey, token, TimeSpan.FromDays(ApplicationConstants.ExpirationDate));
        return token;
    }
}