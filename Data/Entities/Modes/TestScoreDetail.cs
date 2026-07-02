namespace Data.Entities.Modes;

public class TestScoreDetail
{
    public Guid ScoreId { get; set; }
    public Score Score { get; set; } = null!;
}
