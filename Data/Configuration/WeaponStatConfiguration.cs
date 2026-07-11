using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Entities;

namespace Data.Configuration;

public class WeaponStatConfiguration : IEntityTypeConfiguration<WeaponStat>
{
    public void Configure(EntityTypeBuilder<WeaponStat> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Score)
            .WithMany(x => x.WeaponStats)
            .HasForeignKey(x => x.ScoreId);

        builder.HasOne(x => x.Weapon)
            .WithMany(x => x.WeaponStats)
            .HasForeignKey(x => x.WeaponKey);

        builder.Property(x => x.Accuracy).IsRequired(false);
        builder.Property(x => x.CriticalAccuracy).IsRequired(false);
    }
}