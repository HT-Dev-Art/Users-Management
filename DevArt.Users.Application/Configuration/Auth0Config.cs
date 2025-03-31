namespace DevArt.Users.Application.Configuration;

public class Auth0Config
{
    public string ClientId { get; set; } = string.Empty;
    
    public string ClientSecret { get; set; } = string.Empty;
    
    public string GrantType { get; set; } = string.Empty;

    public string OAuthTokenUrl { get; set; } = string.Empty;

    public string ManagementEndPoint { get; set; } = string.Empty;
}
