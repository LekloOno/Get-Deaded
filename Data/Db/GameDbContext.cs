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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GameDbContext).Assembly);

        modelBuilder.Entity<Map>().HasData(
            new Map { MapKey = "dust_pit" }
        );

        modelBuilder.Entity<Weapon>().HasData(
            new Weapon { WeaponKey = "p3_w" },
            new Weapon { WeaponKey = "g0z_brt" }
        );
    }
}