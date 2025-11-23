using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class UIW_PlayerStat : UIW_Stats
{
    [Export] private UIW_WeaponStat _weaponStatTemplate;
    private List<UIW_WeaponStat> _weaponsStats = [];
    private STAT_Combat _combatStats;

    public void Initialize(STAT_Combat combat)
    {
        _combatStats = combat;

        if (_weaponStatTemplate.GetParent() is Node parent)
            parent.RemoveChild(_weaponStatTemplate);

        foreach (UIW_WeaponStat w in _weaponsStats)
            w.QueueFree();
        
        _weaponsStats.Clear();

        foreach (STAT_Weapon weapon in combat.Weapons)
        {
            UIW_WeaponStat stat = (UIW_WeaponStat) _weaponStatTemplate.Duplicate();
            stat.Initialize(weapon);
            _weaponsStats.Add(stat);
            AddChild(stat);
        }
    }
}