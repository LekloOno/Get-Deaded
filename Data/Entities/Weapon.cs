namespace Data.Entities;

public class Weapon
{
    public string WeaponKey { get; set; } = null!;

    public ICollection<WeaponStat> WeaponStats { get; set; } = [];
}