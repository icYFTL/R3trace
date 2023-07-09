using Computantis.database.models;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Computantis.database;

public class ApplicationContext : DbContext
{
    public virtual DbSet<Nationality> Nationalities { get; init; }
    public virtual DbSet<Team> Teams { get; init; }
    public virtual DbSet<User> Users { get; init; }
    public virtual DbSet<RefreshToken> RefreshTokens { get; init; }

    public virtual DbSet<WordsBlackList> WordsBlackList { get; init; }
//    public virtual DbSet<UserInTeam> UserInTeams { get; init; }

    private readonly IConfiguration _configuration;

    public ApplicationContext(IConfiguration configuration)
    {
        _configuration = configuration;
        Database.EnsureCreated();
        _createNationalities();
    }

    public NpgsqlConnection GetNewConnection()
    {
        return new NpgsqlConnection(_configuration.GetConnectionString("Postgres"));
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

            e.Property(x => x.Uid)
                .HasColumnName("uid")
                .HasDefaultValueSql("uuid_generate_v4()");
            e.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(256);
        });
        modelBuilder.Entity<Team>(e =>
        {
            e.HasKey(x => x.Uid);
            e.ToTable("teams");

            e.Property(x => x.Uid)
                .HasColumnName("uid")
                .HasDefaultValueSql("uuid_generate_v4()");
            e.Property(x => x.Name)
                .HasColumnName("name")
                .HasMaxLength(256);
            e.Property(x => x.Code)
                .HasColumnName("code")
                .HasMaxLength(256);
            e.Property(x => x.Trusted)
                .HasColumnName("trusted")
                .HasDefaultValueSql("true");
            e.Property(x => x.OwnerUid)
                .HasColumnName("owner_uid");
            e.Property(x => x.Deleted)
                .HasDefaultValueSql("false")
                .HasColumnName("deleted");
            e.Property(x => x.CreateDate)
                .HasDefaultValueSql("now()")
                .HasColumnName("create_date");
        });
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Uid);
            e.ToTable("users");

            e.Property(x => x.Uid)
                .HasColumnName("uid")
                .HasDefaultValueSql("uuid_generate_v4()");
            e.Property(x => x.Username)
                .HasColumnName("username")
                .HasMaxLength(32);
            e.Property(x => x.Password)
                .HasColumnName("password")
                .HasMaxLength(128);
            e.Property(x => x.Salt)
                .HasColumnName("salt")
                .HasMaxLength(32);
            e.Property(x => x.NationalityUid)
                .HasColumnName("nationality_uid");
            e.Property(x => x.TeamUid)
                .HasColumnName("team_uid");
            e.Property(x => x.Trusted)
                .HasColumnName("trusted")
                .HasDefaultValueSql("true");
            e.Property(x => x.IsAdmin)
                .HasColumnName("is_admin")
                .HasDefaultValueSql("false");
            e.Property(x => x.Banned)
                .HasColumnName("banned")
                .HasDefaultValueSql("false");
            e.Property(x => x.RegisteredIp)
                .HasColumnName("registered_ip");
            e.Property(x => x.LastAccessIp)
                .HasColumnName("last_access_ip");
            e.Property(x => x.Deleted)
                .HasDefaultValueSql("false")
                .HasColumnName("deleted");
            e.Property(x => x.CreateDate)
                .HasDefaultValueSql("now()")
                .HasColumnName("create_date");
            e.Property(x => x.LastAccessDate)
                .HasDefaultValueSql("now()")
                .HasColumnName("last_access_date");

            e.HasOne(x => x.Team)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.TeamUid)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(x => x.Uid);
            entity.ToTable("refresh_tokens");

            entity.Property(e => e.Uid)
                .HasColumnName("uid")
                .HasDefaultValueSql("uuid_generate_v4()");

            entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires");
            entity.Property(e => e.CreatedIp).HasColumnName("created_ip");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.RevokedIp).HasColumnName("revoked_ip");
            entity.Property(e => e.RevokedAt).HasColumnName("revoked_at");
            entity.Property(e => e.Token).HasColumnName("token");
            entity.Property(e => e.UserUid).HasColumnName("user_uid");
        });
        modelBuilder.Entity<WordsBlackList>(entity =>
        {
            entity.HasKey(x => x.Uid);
            entity.ToTable("words_black_list");

            entity.Property(x => x.Uid)
                .HasColumnName("uid")
                .HasDefaultValueSql("uuid_generate_v4()");
            entity.Property(x => x.Word)
                .HasColumnName("word")
                .HasMaxLength(128);

            entity.Property(x => x.Description)
                .HasColumnName("description")
                .HasMaxLength(256);
        });
    }
}