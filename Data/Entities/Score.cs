namespace Data.Entities;

public class Score
{
    public Guid Id { get; set; }

    public DateTime CreatedAt { get; set; }
    
    public string VersionKey { get; set; } = null!;
    public GameVersion GameVersion { get; set; } = null!;

    public Guid PlayerId { get; set; }
    public Player Player { get; set; } = null!;

    public string MapKey { get; set; } = null!;
    public Map Map { get; set; } = null!;

    public string ModeKey { get; set; } = null!;
    public GameMode Mode { get; set; } = null!;

    public int Value { get; set; }
    public int Difficulty { get; set; } // 0-2
    public int TimeMs { get; set; }

    public ICollection<WeaponStat> WeaponStats { get; set; } = [];
}