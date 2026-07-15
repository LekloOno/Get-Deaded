namespace Data.Entities;

public class GameMode
{
    public string ModeKey { get; set; } = null!;

    public ICollection<Score> Scores { get; set; } = [];
}