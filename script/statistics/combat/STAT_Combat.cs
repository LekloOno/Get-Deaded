using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class STAT_Combat : Node
{
    [Export] private PW_WeaponsHandler _weaponsHandler;
    public List<STAT_Weapon> Weapons {get; private set;}
    public float Damage => Weapons.Sum(weapon => weapon.Damage);
    public int Kills => Weapons.Sum(weapon => weapon.Kills);

    public Action GotInitialized;
    public bool Initialized {get; private set;} = false;

    private void Initialize(Godot.Collections.Array<PW_Weapon> weapons)
    {
        Weapons = weapons
            .Select(weapon => new STAT_Weapon(weapon))
            .ToList();
        
        GotInitialized?.Invoke();
    }

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