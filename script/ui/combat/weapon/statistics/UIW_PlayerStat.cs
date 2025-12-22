using System;
using System.Collections.Generic;
using Godot;

namespace Pew;

[GlobalClass]
public partial class UIW_PlayerStat : UIW_Stats
{
    [Export] private UIW_WeaponStat _weaponStatTemplate;
    [Export] private Label _damageLabel;
    [Export] private Label _killsLabel;
    [Export] private Label _deathsLabel;

    private List<UIW_WeaponStat> _weaponsStats = [];
    private STAT_Combat _combatStats;

    private void UpdateDamage(float value) {_damageLabel.Text = Mathf.Floor(value) + "";}
    private void UpdateKills(int value) {_killsLabel.Text = value + "";}
    private void UpdateDeaths(int value) {_deathsLabel.Text = value + "";}

    public void Initialize(STAT_Combat combat)
    {
        _combatStats = combat;

        
        combat.Damage.Subscribe(UpdateDamage);
        combat.Kills.Subscribe(UpdateKills);
        combat.Deaths.Subscribe(UpdateDeaths);

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

    public override void _ExitTree()
    {
        if (_combatStats == null)
            return;
            
        _combatStats.Damage.Unsubscribe(UpdateDamage);
        _combatStats.Kills.Unsubscribe(UpdateKills);
        _combatStats.Deaths.Unsubscribe(UpdateDeaths);
    }
}