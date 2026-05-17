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
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GameDbContext).Assembly);
    }
}