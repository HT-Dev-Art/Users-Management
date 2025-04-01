using System.Text.Json.Serialization;

namespace DevArt.Users.Application.Dto.Auth0;

public class Auth0ResponseDto
{
    public string UserId { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public bool EmailVerified { get; set; }

    public string Username { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public bool PhoneVerified { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }

    public Dictionary<string, object> AppMetadata { get; set; } = new();

    public Dictionary<string, object> UserMetadata { get; set; } = new();

    public string Picture { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Nickname { get; set; } = string.Empty;

    public string LastIp { get; set; } = string.Empty;

    public DateTimeOffset LastLogin { get; set; }

    public int LoginsCount { get; set; }

    public bool Blocked { get; set; }

    public string GivenName { get; set; } = string.Empty;

    public string FamilyName { get; set; } = string.Empty;
}