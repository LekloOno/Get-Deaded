namespace Data.Entities;

public class Score
{
    public Guid Id { get; set; }

    public Guid PlayerId { get; set; }
    public Player Player { get; set; } = null!;

    public string MapId { get; set; } = null!;
    public Map Map { get; set; } = null!;

    public int Value { get; set; }
    public int Difficulty { get; set; } // 0-2
    public int TimeMs { get; set; }

    public ICollection<WeaponStat> WeaponStats { get; set; } = [];
}