namespace DevArt.Users.Application.Dto;

public class UpdateUserDto
{
    public string? NewPassword { get; set; }

    public string? PasswordConfirmation { get; set; }

    public string? NickName { get; set; } 
}