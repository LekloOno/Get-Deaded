namespace Data.Entities;

public class ScoreCompatibilityGroup
{
    public string GroupKey { get; set; } = null!;

    public ICollection<GameVersion> Versions { get; set; } = [];
}