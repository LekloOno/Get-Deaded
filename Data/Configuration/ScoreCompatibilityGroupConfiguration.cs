using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Entities;

namespace Data.Configuration;

public class ScoreCompatibilityGroupConfiguration : IEntityTypeConfiguration<ScoreCompatibilityGroup>
{
    public void Configure(EntityTypeBuilder<ScoreCompatibilityGroup> builder)
    {
        builder.HasKey(x => x.GroupKey);
        builder.Property(x => x.GroupKey).HasMaxLength(64).IsRequired();
    }
}