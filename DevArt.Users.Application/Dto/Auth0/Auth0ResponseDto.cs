using System.Text.Json.Serialization;

namespace DevArt.Users.Application.Dto.Auth0;

public class Auth0ResponseDto
{
    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; set; } = string.Empty;

    [JsonPropertyName("email_verified")]
    public bool EmailVerified { get; set; }

    [JsonPropertyName("username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("phone_number")]
    public string PhoneNumber { get; set; } = string.Empty;

    [JsonPropertyName("phone_verified")]
    public bool PhoneVerified { get; set; }

    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; set; }

    [JsonPropertyName("app_metadata")]
    public Dictionary<string, object> AppMetadata { get; set; } = new();

    [JsonPropertyName("user_metadata")]
    public Dictionary<string, object> UserMetadata { get; set; } = new();

    [JsonPropertyName("picture")]
    public string Picture { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("nickname")]
    public string Nickname { get; set; } = string.Empty;

    [JsonPropertyName("last_ip")]
    public string LastIp { get; set; } = string.Empty;

    [JsonPropertyName("last_login")]
    public DateTimeOffset LastLogin { get; set; }

    [JsonPropertyName("logins_count")]
    public int LoginsCount { get; set; }

    [JsonPropertyName("blocked")]
    public bool Blocked { get; set; }

    [JsonPropertyName("given_name")]
    public string GivenName { get; set; } = string.Empty;

    [JsonPropertyName("family_name")]
    public string FamilyName { get; set; } = string.Empty;
}