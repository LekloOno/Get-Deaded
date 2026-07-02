namespace Data.Entities;

public class Score
{
    public Guid Id { get; set; }

    public Guid PlayerId { get; set; }
    public Player Player { get; set; } = null!;

    public string MapKey { get; set; } = null!;
    public Map Map { get; set; } = null!;

    public string ModeKey { get; set; } = null!;
    public GameMode Mode { get; set; } = null!;

    public string ClientVersionKey { get; set; } = null!;
    public ClientVersion ClientVersion { get; set; } = null!;


    public int Value { get; set; }
    public int Difficulty { get; set; } // 0-2
    public int TimeMs { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<WeaponStat> WeaponStats { get; set; } = [];
}