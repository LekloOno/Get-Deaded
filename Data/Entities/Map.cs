namespace Data.Entities;

public class Map
{
    public string MapKey { get; set; } = null!;

    public ICollection<Score> Scores { get; set; } = [];
}