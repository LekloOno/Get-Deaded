namespace Data.Entities;

public class WeaponStat
{
    public Guid Id { get; set; }

    public string WeaponId { get; set; } = null!;
    public Weapon Weapon { get; set; } = null!;

    public Guid ScoreId { get; set; }
    public Score Score { get; set; } = null!;

    public int TotalDamage { get; set; }
    public int TotalKills { get; set; }
    public float Accuracy { get; set; }
    public float CriticalAccuracy { get; set; }
}