using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Entities;

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
            .HasForeignKey(x => x.MapId);

        builder.Property(x => x.Difficulty)
            .IsRequired();

        builder.Property(x => x.TimeMs)
            .IsRequired();
    }
}