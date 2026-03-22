using Microsoft.EntityFrameworkCore;
using ServerHost.Domain.Entities;

namespace ServerHost.Infrastructure.Persistance;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<GameServer> Servers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<GameServer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.HasIndex(e => e.Name).IsUnique();
            entity.Property(e => e.Port).IsRequired();
            entity.HasIndex(e => e.Port).IsUnique();
        });
    }
}
