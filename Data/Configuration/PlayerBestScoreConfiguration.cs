using Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Configuration;

public class PlayerBestScoreConfiguration : IEntityTypeConfiguration<PlayerBestScore>
{
    public void Configure(EntityTypeBuilder<PlayerBestScore> builder)
    {
        builder.ToTable("player_best_scores");

        builder.HasKey(x => new
        {
            x.PlayerId,
            x.MapKey,
            x.Difficulty,
            x.ModeKey
        });

        builder.HasOne(x => x.Score)
            .WithOne()
            .HasForeignKey<PlayerBestScore>(x => x.ScoreId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ScoreId).IsUnique();
    }
}