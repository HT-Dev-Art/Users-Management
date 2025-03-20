using System.IdentityModel.Tokens.Jwt;
using DevArt.Users.Application.Configuration;
using DevArt.Users.Application.Dto;
using DevArt.Users.Application.Dto.Auth0;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Authenticators;

namespace DevArt.Users.Application.Service.Impl;

public class Auth0Service(IOptionsSnapshot<Auth0Config> auth0ConfigSnapshot) : IAuth0Service
{
    private string _auth0Token = string.Empty;
    private readonly Auth0Config _auth0Config = auth0ConfigSnapshot.Value;
    
    private async Task<string> GetAut0Token()
    {
        if (_auth0Token == string.Empty)
        {
            _auth0Token = await RefreshAuth0Token();
        }
        
        else
        {
            var jwt = new JwtSecurityTokenHandler();
            var token = jwt.ReadToken(_auth0Token);
            if (token.ValidFrom < DateTime.Now)
            {
                _auth0Token = await RefreshAuth0Token();
            }
        }

        return _auth0Token;
    }

    private async Task<string> RefreshAuth0Token()
    {
        var client = new RestClient(_auth0Config.OAuthTokenUrl);
        var request = new RestRequest("",Method.Post);
        
        request.AddHeader("content-type", "application/json");
        request.AddParameter("audience", _auth0Config.Audience, ParameterType.RequestBody);
        request.AddParameter("client_id", _auth0Config.ClientId, ParameterType.RequestBody);
        request.AddParameter("client_secret", _auth0Config.ClientSecret, ParameterType.RequestBody);
        request.AddParameter("grant_type", _auth0Config.GrantType, ParameterType.RequestBody);
        var result = await client.PostAsync<Auth0CredentialDto>(request);
        return result?.AccessToken ?? "";
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
        request.AddParameter("connection", Constants.Auth0Constant.Connection, ParameterType.RequestBody);
        request.AddParameter("email", userDto.Email, ParameterType.RequestBody);
        request.AddParameter("password", userDto.Password, ParameterType.RequestBody);
        if (userDto.NickName != null)
        {
            request.AddParameter("password", ParameterType.RequestBody);
        }

        var result = await client.PostAsync<Auth0ResponseDto>(request);
        return result ?? new Auth0ResponseDto();
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
        foreach (var key in bodyDictionary.Keys)
        {
            if (bodyDictionary[key] != null)
            {
                request.AddParameter(key, bodyDictionary[key]!, ParameterType.RequestBody);
            }
        }

        var result = await client.PostAsync<Auth0ResponseDto>(request);
        return result ?? new Auth0ResponseDto();
    }
}