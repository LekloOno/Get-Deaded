using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class UI_WeaponsLoadout : Control
{
    [Export] private PW_WeaponsHandler _weaponsHandler;
    [Export] private UI_UnactiveWeaponsList _list;
    [Export] private UI_WeaponHolder _active;
    [Export] private UI_WeaponHolder _holster;
    public override void _Ready()
    {
        _weaponsHandler.SwitchStarted += HandleSwitch;
        _weaponsHandler.Initialized += Initialize;

        _weaponsHandler.InitData(out PW_Weapon active, out PW_Weapon nextHolster, out int nextIndex, out Array<PW_Weapon> weapons);
        if (active == null)
            return;

        Initialize(active, nextHolster, nextIndex, weapons);
    }

    private void Initialize(PW_Weapon active, PW_Weapon nextHolster, int nextIndex, Array<PW_Weapon> weapons)
    {
        _active.Initialize(active);
        _holster.Initialize(nextHolster);
        _list.InitializeWeapons(weapons, nextIndex);
    }

    private void HandleSwitch(PW_Weapon active, PW_Weapon nextHolster, int nextIndex, Array<PW_Weapon> weapons)
    {
        _active.Initialize(active);
        _holster.Initialize(nextHolster);
        _list.SetWeapons(weapons, nextIndex);
    }
}