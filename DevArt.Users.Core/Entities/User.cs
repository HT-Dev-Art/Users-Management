using DevArt.Core.Modelling;

namespace DevArt.Users.Core.Entities;

public class User : IEntityModel<int, byte[]>
{
    public int Id { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    
    public DateTimeOffset UpdatedAt { get; set; }
    
    public byte[] RowVersion { get; }

    public string Auth0Id { get; set; } = string.Empty;
    
}