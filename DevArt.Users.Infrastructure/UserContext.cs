using System.Reflection;
using DevArt.Users.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevArt.Users.Infrastructure;

public class UserContext(DbContextOptions<UserContext> options) : DbContext(options: options)
{
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserContext).Assembly);
        
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var currentTime = DateTimeOffset.UtcNow;
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Property("CreatedAt").CurrentValue = currentTime;
                entry.Property("UpdatedAt").CurrentValue = currentTime;
            }

            if (entry.State == EntityState.Modified)
            {

                entry.Property("UpdatedAt").CurrentValue = currentTime;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    public Task<int> BaseSaveChangeAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return base.SaveChangesAsync(cancellationToken);
    }
}