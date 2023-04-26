using Computantis.database.models;
using Microsoft.EntityFrameworkCore;
using R3TraceShared.utils;

namespace Computantis.database;

public class ApplicationContext : DbContext
{
    public virtual DbSet<Nationality> Nationalities { get; init; }
    public virtual DbSet<Team> Teams { get; init; }
    public virtual DbSet<User> Users { get; init; }
//    public virtual DbSet<UserInTeam> UserInTeams { get; init; }

    private readonly IConfiguration _configuration;

    public ApplicationContext(IConfiguration configuration)
    {
        _configuration = configuration;
        Database.EnsureCreated();
        _createNationalities();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_configuration.GetConnectionString("Postgres"));
    }

    private void _createNationalities()
    {
        if (!Nationalities.Any())
        {
            // TODO: Localization
            var countries = File.ReadAllLines("static/countries/en.txt"); 
            Nationalities.AddRange(countries.Select((x, i) => new Nationality
            {
                Name = x
            }));
            SaveChanges();
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Nationality>(e =>
        {
            e.HasKey(x => x.Uid);
            e.ToTable("nationalities");

            e.Property(x => x.Uid).HasDefaultValueSql("uuid_generate_v4()");
            e.Property(x => x.Name).HasMaxLength(256);
        });
        modelBuilder.Entity<Team>(e =>
        {
            e.HasKey(x => x.Uid);
            e.ToTable("teams");

            e.Property(x => x.Uid).HasDefaultValueSql("uuid_generate_v4()");
            e.Property(x => x.Name).HasMaxLength(256);
            e.Property(x => x.Code).HasMaxLength(64);
        });
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Uid);
            e.ToTable("users");

            e.Property(x => x.Uid).HasDefaultValueSql("uuid_generate_v4()");
            e.Property(x => x.Username).HasMaxLength(32);
            e.Property(x => x.Password).HasMaxLength(128);
            e.Property(x => x.Salt).HasMaxLength(32);
            
            e.HasOne(x => x.Team)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.TeamUid)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}