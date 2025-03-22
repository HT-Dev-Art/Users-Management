using System.Text.Json.Serialization;

namespace DevArt.Users.Application.Dto.Auth0;

public class Auth0CredentialDto
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; } = string.Empty;
}