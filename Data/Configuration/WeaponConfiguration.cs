using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Entities;

namespace Data.Configuration;

public class WeaponConfiguration : IEntityTypeConfiguration<Weapon>
{
    public void Configure(EntityTypeBuilder<Weapon> builder)
    {
        builder.HasKey(x => x.WeaponKey);

        builder.Property(x => x.WeaponKey)
            .HasMaxLength(64)
            .IsRequired();
    }
}