namespace Data.Entities;

public class GameVersion
{
    public string VersionString { get; set; } = null!;

    public string GroupKey { get; set; } = null!;
    public ScoreCompatibilityGroup Group { get; set; } = null!;

    public DateTime ReleasedAt { get; set; }

    public ICollection<Score> Scores { get; set; } = [];
}