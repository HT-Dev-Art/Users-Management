using DevArt.Users.Application.Configuration;
using DevArt.Users.Application.Dto;
using DevArt.Users.Application.Dto.Auth0;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators;

namespace DevArt.Users.Application.Service.Impl;

public class Auth0Service(IOptions<Auth0Config> auth0ConfigSnapshot) : IAuth0Service
{
    private string _auth0Token = string.Empty;
    private DateTimeOffset? _tokenExpireTime;
    private readonly Auth0Config _auth0Config = auth0ConfigSnapshot.Value;
    
    private async Task<string> GetAut0Token()
    {
        if (_auth0Token == string.Empty)
        {
            _auth0Token = await RefreshAuth0Token();
            _tokenExpireTime = DateTimeOffset.UtcNow.AddDays(1);
        }
        
        else
        {
            if (_tokenExpireTime < DateTimeOffset.UtcNow)
            {
                _auth0Token = await RefreshAuth0Token();
                _tokenExpireTime = DateTimeOffset.UtcNow.AddDays(1);
            }
        }

        return _auth0Token;
    }

    private async Task<string> RefreshAuth0Token()
    {
        var client = new RestClient(_auth0Config.OAuthTokenUrl);

        var body = new Dictionary<string, string>
        {
            { "audience", _auth0Config.ManagementEndPoint },
            { "client_id", _auth0Config.ClientId },
            { "client_secret", _auth0Config.ClientSecret },
            { "grant_type", _auth0Config.GrantType }
        };
        var request = new RestRequest();
        request.AddHeader("content-type", "application/json");
        request.AddJsonBody(body);
        var result = await client.ExecutePostAsync<Auth0CredentialDto>(request);
            
        return  result.Data?.AccessToken ?? "";
    }


    public async Task<Auth0ResponseDto> CreateUser(CreateUserDto userDto)
    {
        var credential = await GetAut0Token();
        var clientOption = new RestClientOptions()
        {
            Authenticator = new JwtAuthenticator(credential),
            BaseUrl = new Uri(_auth0Config.ManagementEndPoint)
        };
        var client = new RestClient( clientOption);
        var request = new RestRequest("users", Method.Post);
        var body = new Dictionary<string, string>
        {
            { "connection", Constants.Auth0Constant.Connection },
            { "email", userDto.Email },
            { "password", userDto.Password }
        };
        if (userDto.NickName != null)
        {
            body["nickname"] = userDto.NickName;
        }

        request.AddJsonBody(body);
        
        var result = await client.ExecutePostAsync<Auth0ResponseDto>(request);
        return result.Data ?? new Auth0ResponseDto();
    }

    public async Task<Auth0ResponseDto> UpdateUser(UpdateUserDto updateUserDto, string auth0Id)
    {
        var credential = await GetAut0Token();
        var clientOption = new RestClientOptions()
        {
            Authenticator = new JwtAuthenticator(credential),
            BaseUrl = new Uri(_auth0Config.ManagementEndPoint)
        };
        var client = new RestClient( clientOption);
        var request = new RestRequest($"users\\{auth0Id}", Method.Patch);
        
        var bodyDictionary = new Dictionary<string, string?>
            { { "password", updateUserDto.NewPassword }, {"nickname", updateUserDto.NickName} };
        
        foreach (var key in bodyDictionary.Keys.Where(key => bodyDictionary[key] == null))
        {
            bodyDictionary.Remove(key);
        }
        
        request.AddJsonBody(bodyDictionary);

        var result = await client.ExecutePatchAsync<Auth0ResponseDto>(request);
        return result.Data ?? new Auth0ResponseDto();
    }

    public async Task<RestResponse> UpdateUserRole(string roleId, List<string> auth0Ids) 
    {
        var credential = await GetAut0Token();
        var clientOption = new RestClientOptions()
        {
            Authenticator = new JwtAuthenticator(credential),
            BaseUrl = new Uri(_auth0Config.ManagementEndPoint)
        };
        var client = new RestClient( clientOption);
        var request = new RestRequest($"roles\\{roleId}\\users", Method.Post);
        var body = new Dictionary<string, List<string>> { { "users", auth0Ids } };
        request.AddJsonBody(body);
        var response = await client.ExecutePostAsync(request: request);
        return response;
    }

    public async Task<RestResponse> DeleteUser(string auth0Id)
    {
        var credential = await GetAut0Token();
        var clientOption = new RestClientOptions()
        {
            Authenticator = new JwtAuthenticator(credential),
            BaseUrl = new Uri(_auth0Config.ManagementEndPoint)
        };
        var client = new RestClient(clientOption);
        var request = new RestRequest($"users\\{auth0Id}", Method.Delete);
        var response = await client.ExecutePostAsync(request: request);
        return response;
    }
}