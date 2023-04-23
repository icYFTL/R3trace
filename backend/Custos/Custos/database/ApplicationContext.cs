using Custos.database.models;
using Microsoft.EntityFrameworkCore;

namespace Custos.database;

public class ApplicationContext : DbContext
{
    public virtual DbSet<Ctf> Ctfs { get; init; } = null!;

    private readonly IConfiguration _configuration;

    public ApplicationContext(IConfiguration configuration)
    {
        _configuration = configuration;
        Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("Postgres"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ctf>(e =>
        {
            e.HasKey(x => x.Uid);
            e.ToTable("ctfs");

            e.Property(x => x.Uid).HasDefaultValueSql("uuid_generate_v4()");
            e.Property(x => x.Name).HasMaxLength(256);
            e.Property(x => x.Title).HasMaxLength(256);
            e.Property(x => x.Code).HasMaxLength(100);
        });
    }
}