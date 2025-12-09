using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class STAT_CombatTracker : Node
{
    [Export] private PW_WeaponsHandler _weaponsHandler;
    [Export] private GC_HealthManager _healthManager;

    public STAT_Combat Data {get; private set;} = null;

    public List<STAT_Weapon> Weapons => Data.Weapons;
    public STAT_Weapon MeleeWeapon => Data.MeleeWeapon;
    public Observable<float> Damage => Data.Damage;
    public Observable<int> Kills => Data.Kills;
    public Observable<int> Deaths => Data.Deaths;

    public Action GotInitialized;
    public bool Initialized => Data != null;

    private void Initialize(Godot.Collections.Array<PW_Weapon> weapons) =>
        Data = new(weapons, _weaponsHandler.Melee, _healthManager);

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