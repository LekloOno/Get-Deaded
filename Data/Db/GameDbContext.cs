using Data.Entities;
using Data.Entities.Modes;
using Microsoft.EntityFrameworkCore;

namespace Data.Db;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options)
        : base(options) {}

    public DbSet<Player>            Players => Set<Player>();
    public DbSet<Map>               Maps => Set<Map>();
    public DbSet<Weapon>            Weapons => Set<Weapon>();
    public DbSet<Score>             Scores => Set<Score>();
    public DbSet<PlayerBestScore>   BestScores => Set<PlayerBestScore>();
    public DbSet<WeaponStat>        WeaponStats => Set<WeaponStat>();
    public DbSet<TestScoreDetail>   TestScores => Set<TestScoreDetail>();
    public DbSet<GameMode>          GameModes => Set<GameMode>();
    public DbSet<ClientVersion>     GameVersions => Set<ClientVersion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GameDbContext).Assembly);

        modelBuilder.Entity<Map>().HasData(
            new Map { MapKey = "dust_pit" }
        );

        modelBuilder.Entity<GameMode>().HasData(
            new GameMode { ModeKey = "test" }
        );

        modelBuilder.Entity<Weapon>().HasData(
            new Weapon { WeaponKey = "p3_w" },
            new Weapon { WeaponKey = "g0z_brt" },
            new Weapon { WeaponKey = "fists" }
        );

        modelBuilder.Entity<Player>().HasData(
            new Player
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                Username = "test",
                PasswordHash = "$2a$12$dWEi/DtIMdGlLsLFirWgfOzsrPK8dFjWGPsexIZE3cUhW1Yqi/DmO"
            }
        );
    }
}