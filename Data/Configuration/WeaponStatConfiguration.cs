using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Entities;

public class WeaponStatConfiguration : IEntityTypeConfiguration<WeaponStat>
{
    public void Configure(EntityTypeBuilder<WeaponStat> builder)
    {
        builder.ToTable("weapon_stats");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Score)
            .WithMany(x => x.WeaponStats)
            .HasForeignKey(x => x.ScoreId);

        builder.HasOne(x => x.Weapon)
            .WithMany(x => x.WeaponStats)
            .HasForeignKey(x => x.WeaponId);

        builder.Property(x => x.Accuracy).IsRequired();
        builder.Property(x => x.CriticalAccuracy).IsRequired();
    }
}