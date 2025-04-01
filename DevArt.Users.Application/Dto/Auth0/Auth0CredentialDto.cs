namespace DevArt.Users.Application.Dto.Auth0;

public class Auth0CredentialDto
{
    public string AccessToken { get; set; } = string.Empty;

    public string TokenType { get; set; } = string.Empty;
}