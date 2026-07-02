using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Data.Entities.Modes;

namespace Data.Configuration.Modes;

public class TestScoreDetailConfiguration : IEntityTypeConfiguration<TestScoreDetail>
{
    public void Configure(EntityTypeBuilder<TestScoreDetail> builder)
    {
        builder.ToTable("test_score_details");

        builder.HasKey(x => x.ScoreId);

        builder.HasOne(x => x.Score)
            .WithOne()
            .HasForeignKey<TestScoreDetail>(x => x.ScoreId);
    }
}