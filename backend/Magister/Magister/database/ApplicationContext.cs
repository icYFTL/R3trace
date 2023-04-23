using Magister.database.models;
using Microsoft.EntityFrameworkCore;

namespace Magister.database;

public class ApplicationContext : DbContext
{
    public virtual DbSet<CtfTask> Tasks { get; init; }
    public virtual DbSet<TaskType> TaskTypes { get; init; }

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
        modelBuilder.Entity<CtfTask>(e =>
        {
            e.HasKey(x => x.Uid);
            e.ToTable("tasks");

            e.Property(x => x.Uid).HasDefaultValueSql("uuid_generate_v4()");
            e.Property(x => x.Name).HasMaxLength(256);
            e.Property(x => x.Description).HasMaxLength(1024);
            e.Property(x => x.Visible).HasDefaultValueSql("false");
        });

        modelBuilder.Entity<TaskType>(e =>
        {
            e.HasKey(x => x.Uid);
            e.ToTable("tasks_types");

            e.Property(x => x.Uid).HasDefaultValueSql("uuid_generate_v4()");
            e.Property(x => x.Name).HasMaxLength(256);
            e.Property(x => x.Description).HasMaxLength(1024);
        });
    }
}