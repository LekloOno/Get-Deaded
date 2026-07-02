namespace Data.Entities;

public class PlayerBestScore
{
    public Guid PlayerId { get; set; }

    public Guid ScoreId { get; set; }
    public Score Score { get; set; } = null!;

    public string MapKey { get; set; } = null!;
    public int Difficulty { get; set; }
    public string ModeKey { get; set; } = null!;

    public int Value { get; set; }
}