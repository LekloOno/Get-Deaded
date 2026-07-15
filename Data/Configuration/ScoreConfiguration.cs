using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Entities;

namespace Data.Configuration;

public class ScoreConfiguration : IEntityTypeConfiguration<Score>
{
    public void Configure(EntityTypeBuilder<Score> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(s => s.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(x => x.GameVersion)
            .WithMany(x => x.Scores)
            .HasForeignKey(x => x.VersionKey);

        builder.HasOne(x => x.Player)
            .WithMany(x => x.Scores)
            .HasForeignKey(x => x.PlayerId);

        builder.HasOne(x => x.Map)
            .WithMany(x => x.Scores)
            .HasForeignKey(x => x.MapKey);

        builder.HasOne(x => x.Mode)
            .WithMany(x => x.Scores)
            .HasForeignKey(x => x.ModeKey);

        builder.Property(x => x.Difficulty)
            .IsRequired();

        builder.Property(x => x.TimeMs)
            .IsRequired();

        builder.HasIndex(x => new { x.MapKey, x.ModeKey, x.Difficulty, x.PlayerId, x.Value })
            .IsDescending(false, false, false, false, true)
            .HasDatabaseName("ix_scores_leaderboard_lookup");
    }
}