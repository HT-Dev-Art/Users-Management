namespace DevArt.Users.Application.Dto;

public class CreateUserDto
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string PasswordConfirmation { get; set; } = string.Empty;
    
    public string? NickName { get; set; }
    
}