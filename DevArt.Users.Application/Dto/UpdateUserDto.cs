namespace DevArt.Users.Application.Dto;

public class UpdateUserDto
{
    public string? NewPassword { get; set; } = string.Empty;

    public string? PasswordConfirmation { get; set; } = string.Empty;

    public string? NickName { get; set; } = string.Empty;
}