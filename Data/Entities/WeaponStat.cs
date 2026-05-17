namespace Data.Entities;

public class WeaponStat
{
    public Guid Id { get; set; }

    public string WeaponKey { get; set; } = null!;
    public Weapon Weapon { get; set; } = null!;

    public Guid ScoreId { get; set; }
    public Score Score { get; set; } = null!;

    public int Damage { get; set; }
    public int Kills { get; set; }
    public float Accuracy { get; set; }
    public float CriticalAccuracy { get; set; }
}