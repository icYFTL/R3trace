using Custos.database.models;
using Microsoft.EntityFrameworkCore;

namespace Custos.database;

public class ApplicationContext : DbContext
{
    public virtual DbSet<Ctf> Ctfs { get; init; } = null!;
    public virtual DbSet<TeamInCtf> TeamInCtfs { get; init; } = null!;

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

            e.Property(x => x.Uid)
                .HasColumnName("uid")
                .HasDefaultValueSql("uuid_generate_v4()");
            e.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(256);
            e.Property(x => x.Title)
                .HasColumnName("title")
                .HasMaxLength(256);
            e.Property(x => x.Code)
                .HasColumnName("code")
                .HasMaxLength(100);
        });

        modelBuilder.Entity<Setting>(e =>
        {
            e.HasKey(x => x.Uid);
            e.ToTable("settings");

            e.Property(x => x.Uid)
                .HasColumnName("uid")
                .HasDefaultValueSql("uuid_generate_v4()");

            e.Property(x => x.Key)
                .HasColumnName("key")
                .HasMaxLength(128);

            e.Property(x => x.Value)
                .HasColumnName("value")
                .HasMaxLength(1024);
        });

        modelBuilder.Entity<TeamInCtf>(e =>
        {
            e.HasKey(x => x.Uid);
            e.ToTable("user_in_ctf");

            e.Property(x => x.Uid)
                .HasColumnName("uid")
                .HasDefaultValueSql("uuid_generate_v4()");

            e.Property(x => x.TeamUid)
                .HasColumnName("team_uid");

            e.Property(x => x.CtfUid)
                .HasColumnName("ctf_uid");
        });
    }
}