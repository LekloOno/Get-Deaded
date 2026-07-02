using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Entities;

namespace Data.Configuration;

public class ScoreCompatibilityGroupConfiguration : IEntityTypeConfiguration<ScoreCompatibilityGroup>
{
    public void Configure(EntityTypeBuilder<ScoreCompatibilityGroup> builder)
    {
        builder.ToTable("score_compatibility_group");

        builder.HasKey(x => x.Id);
    }
}