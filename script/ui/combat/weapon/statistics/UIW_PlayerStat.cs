using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class UIW_PlayerStat : UIW_Stats
{
    [Export] private UIW_WeaponStat _weaponStatTemplate;
    [Export] private Label _damageLabel;
    [Export] private Label _killsLabel;
    [Export] private Label _deathsLabel;

    private List<UIW_WeaponStat> _weaponsStats = [];
    private STAT_Combat _combatStats;
    private bool _boundToData = false;

    private void UpdateDamage(float value) {_damageLabel.Text = Mathf.Floor(value) + "";}
    private void UpdateKills(int value) {_killsLabel.Text = value + "";}
    private void UpdateDeaths(int value) {_deathsLabel.Text = value + "";}

    public void Initialize(STAT_Combat combat)
    {
        _combatStats = combat;
        
        Bind();

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
        Unbind();
    }

    public override void _EnterTree()
    {
        Bind();
    }


    private void Bind()
    {
        if (_combatStats == null)
            return;

        if (_boundToData)
            return;

        _combatStats.Damage.Subscribe(UpdateDamage);
        _combatStats.Kills.Subscribe(UpdateKills);
        _combatStats.Deaths.Subscribe(UpdateDeaths);
        _boundToData = true;
    }

    private void Unbind()
    {
        if (_combatStats == null)
            return;

        if (!_boundToData)
            return;

        _combatStats.Damage.Unsubscribe(UpdateDamage);
        _combatStats.Kills.Unsubscribe(UpdateKills);
        _combatStats.Deaths.Unsubscribe(UpdateDeaths);
        _boundToData = false;
    }
}