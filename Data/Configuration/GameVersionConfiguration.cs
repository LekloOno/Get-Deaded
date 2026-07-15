using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Entities;

namespace Data.Configuration;

public class GameVersionConfiguration : IEntityTypeConfiguration<GameVersion>
{
    public void Configure(EntityTypeBuilder<GameVersion> builder)
    {
        builder.HasKey(x => x.VersionKey);

        builder.HasOne(x => x.Group)
            .WithMany(x => x.Versions)
            .HasForeignKey(x => x.GroupKey)
            .IsRequired();

        builder.Property(x => x.ReleasedAt).IsRequired();
    }
}