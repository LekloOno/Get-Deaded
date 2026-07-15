using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Db;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options)
        : base(options) {}

    public DbSet<Player> Players => Set<Player>();
    public DbSet<Map> Maps => Set<Map>();
    public DbSet<Weapon> Weapons => Set<Weapon>();
    public DbSet<Score> Scores => Set<Score>();
    public DbSet<WeaponStat> WeaponStats => Set<WeaponStat>();
    public DbSet<GameMode> GameModes => Set<GameMode>();
    public DbSet<GameVersion> GameVersions => Set<GameVersion>();
    public DbSet<ScoreCompatibilityGroup> ScoreCompatibilityGroups => Set<ScoreCompatibilityGroup>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GameDbContext).Assembly);

        modelBuilder.Entity<Map>().HasData(
            new Map { MapKey = "dust_pit" }
        );

        modelBuilder.Entity<Weapon>().HasData(
            new Weapon { WeaponKey = "p3_w" },
            new Weapon { WeaponKey = "g0z_brt" },
            new Weapon { WeaponKey = "fists" }
        );

        modelBuilder.Entity<GameMode>().HasData(
            new GameMode { ModeKey = "test" }
        );

        modelBuilder.Entity<ScoreCompatibilityGroup>().HasData(
            new ScoreCompatibilityGroup { GroupKey = "pre-0.3" },
            new ScoreCompatibilityGroup { GroupKey = "0.3_sectors" }
        );

        modelBuilder.Entity<GameVersion>().HasData(
            new GameVersion { VersionKey = "0.2.4", GroupKey = "pre-0.3", ReleasedAt = new DateTime(2026, 7, 17, 0, 0, 0, DateTimeKind.Utc) }
        );

        modelBuilder.Entity<GameVersion>().HasData(
            new GameVersion { VersionKey = "0.3.0", GroupKey = "0.3_sectors", ReleasedAt = new DateTime(2026, 7, 17, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}