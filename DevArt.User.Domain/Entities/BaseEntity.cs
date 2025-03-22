namespace DevArt.Users.Core.Entities;

public class BaseEntity
{
    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }
}