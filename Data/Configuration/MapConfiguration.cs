using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Entities;

public class MapConfiguration : IEntityTypeConfiguration<Map>
{
    public void Configure(EntityTypeBuilder<Map> builder)
    {
        builder.ToTable("maps");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.MapKey)
            .IsRequired()
            .HasMaxLength(64);

        builder.HasIndex(x => x.MapKey)
            .IsUnique();
    }
}