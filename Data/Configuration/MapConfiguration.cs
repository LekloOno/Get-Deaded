using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Entities;

namespace Data.Configuration;

public class MapConfiguration : IEntityTypeConfiguration<Map>
{
    public void Configure(EntityTypeBuilder<Map> builder)
    {
        builder.ToTable("maps");

        builder.HasKey(x => x.MapKey);

        builder.Property(x => x.MapKey)
            .HasMaxLength(64)
            .IsRequired();
    }
}