using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Entities;

namespace Data.Configuration;

public class GameVersionConfiguration : IEntityTypeConfiguration<ClientVersion>
{
    public void Configure(EntityTypeBuilder<ClientVersion> builder)
    {
        builder.ToTable("game_version");

        builder.HasKey(x => x.VersionKey);

        builder.Property(x => x.AcceptingSubmissions)
            .IsRequired();

        builder.HasOne(x => x.CompatibilityGroup)
            .WithMany(x => x.Versions)
            .HasForeignKey(x => x.CompatibilityGroupId);
    }
}