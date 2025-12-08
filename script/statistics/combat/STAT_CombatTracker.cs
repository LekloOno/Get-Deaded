using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class STAT_CombatTracker : Node
{
    [Export] private PW_WeaponsHandler _weaponsHandler;
    [Export] private GC_HealthManager _healthManager;

    private STAT_Combat _data = null;

    public List<STAT_Weapon> Weapons => _data.Weapons;
    public STAT_Weapon MeleeWeapon => _data.MeleeWeapon;
    public Observable<float> Damage => _data.Damage;
    public Observable<int> Kills => _data.Kills;
    public Observable<int> Deaths => _data.Deaths;

    public Action GotInitialized;
    public bool Initialized => _data != null;

    private void Initialize(Godot.Collections.Array<PW_Weapon> weapons) =>
        _data = new(weapons, _weaponsHandler.Melee, _healthManager);

    private void WeaponsHandlerInitialized(PW_Weapon active, PW_Weapon nextHolster, int nextIndex, Godot.Collections.Array<PW_Weapon> weapons) =>
        Initialize(weapons);
        
    public override void _Ready()
    {
        if (_weaponsHandler.Initialized)
            Initialize(_weaponsHandler.Weapons);
        else    
            _weaponsHandler.GotInitialized += WeaponsHandlerInitialized;
    }
}