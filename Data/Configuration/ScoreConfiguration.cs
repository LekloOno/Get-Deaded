using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Entities;

namespace Data.Configuration;

public class ScoreConfiguration : IEntityTypeConfiguration<Score>
{
    public void Configure(EntityTypeBuilder<Score> builder)
    {
        builder.ToTable("scores");

        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Player)
            .WithMany(x => x.Scores)
            .HasForeignKey(x => x.PlayerId);

        builder.HasOne(x => x.Map)
            .WithMany(x => x.Scores)
            .HasForeignKey(x => x.MapKey);

        builder.HasOne(x => x.Mode)
            .WithMany(x => x.Scores)
            .HasForeignKey(x => x.ModeKey);

        builder.Property(x => x.ClientVersion)
            .IsRequired();

        builder.Property(x => x.Difficulty)
            .IsRequired();

        builder.Property(x => x.TimeMs)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }
}