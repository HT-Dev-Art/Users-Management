namespace DevArt.Users.Core.Entities;

public class User : BaseEntity
{
    public int Id { get; set; }

    public string? UserName { get; set; }

    public string Auth0Id { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    
}