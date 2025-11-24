using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[GlobalClass]
public partial class STAT_Combat : Node
{
    [Export] private PW_WeaponsHandler _weaponsHandler;
    [Export] private GC_HealthManager _healthManager;

    public List<STAT_Weapon> Weapons {get; private set;}
    public STAT_Weapon MeleeWeapon {get; private set;}
    public Observable<float> Damage {get; private set;} = new(0);
    public Observable<int> Kills {get; private set;} = new(0);
    public Observable<int> Deaths {get; private set;} = new(0);

    public Action GotInitialized;
    public bool Initialized {get; private set;} = false;

    private void UpdateDamage(float value) {Damage.Value = Weapons.Sum(weapon => weapon.Damage) + MeleeWeapon.Damage;}
    private void UpdateKills(int value) {Kills.Value = Weapons.Sum(weapon => weapon.Kills) + MeleeWeapon.Kills;}

    private void Initialize(Godot.Collections.Array<PW_Weapon> weapons)
    {
        Weapons = weapons
            .Select(weapon => 
            {
                STAT_Weapon stat = new(weapon);
                stat.Damage.Subscribe(UpdateDamage);
                stat.Kills.Subscribe(UpdateKills);
                return stat;
            })
            .ToList();
        
        MeleeWeapon = new(_weaponsHandler.Melee);
        MeleeWeapon.Damage.Subscribe(UpdateDamage);
        MeleeWeapon.Kills.Subscribe(UpdateKills);
        
        _healthManager.TopHealthLayer.OnDie += CountDeath;
        
        GotInitialized?.Invoke();
        Initialized = true;
    }

    private void CountDeath(GC_Health senderLayer) => Deaths.Value ++;

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