using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class UI_WeaponsLoadout : BoxContainer
{
    [Export] private PW_WeaponsHandler _weaponsHandler;
    [Export] private UIW_Weapon _weaponTemplate;
    [Export] private Container _holster;
    [Export] private Container _active;
    [Export] private Container _unactiveList;

    private Dictionary<PW_Weapon, UIW_Weapon> _weapons = [];
    public override void _Ready()
    {
        _weaponsHandler.SwitchStarted += Update;
        _weaponsHandler.Initialized += Initialize;

        _weaponsHandler.InitData(out PW_Weapon active, out PW_Weapon nextHolster, out int nextIndex, out Godot.Collections.Array<PW_Weapon> weapons);
        if (active == null)
            return;

        Initialize(active, nextHolster, nextIndex, weapons);
    }

    private void Initialize(PW_Weapon active, PW_Weapon nextHolster, int nextIndex, Godot.Collections.Array<PW_Weapon> weapons)
    {
        if (_weaponTemplate.GetParent() == this)
            RemoveChild(_weaponTemplate);

        foreach (UIW_Weapon weapon in _weapons.Values)
            weapon.QueueFree();

        _weapons.Clear();

        InitializeWeapon(active);
        InitializeWeapon(nextHolster);

        int count = weapons.Count;
        for (int i = 0; i < count - 1; i++)
        {
            int realIndex = (i + nextIndex) % count;
            InitializeWeapon(weapons[realIndex]);
        }

        // Initialization could be more efficient by avoiding one dictionnary lookup, since we already know the uiw elements.
        Update(active, nextHolster, nextIndex, weapons);

        //_active.Initialize(active);
        //_holster.Initialize(nextHolster);
        //_list.InitializeWeapons(weapons, nextIndex);
    }

    private void InitializeWeapon(PW_Weapon weapon)
    {
        UIW_Weapon uiWeapon = (UIW_Weapon) _weaponTemplate.Duplicate();
        uiWeapon.Initialize(weapon);
        _weapons.Add(weapon, uiWeapon);
    }

    private void Update(PW_Weapon active, PW_Weapon nextHolster, int nextIndex, Godot.Collections.Array<PW_Weapon> weapons)
    {
        if (_weapons.TryGetValue(active, out UIW_Weapon uiWeapon))
        {
            uiWeapon.SetActive();
            _active.AddChild(uiWeapon);
        }

        if (_weapons.TryGetValue(nextHolster, out uiWeapon))
        {
            uiWeapon.SetUnactive();
            _holster.AddChild(uiWeapon);
        }

        int count = weapons.Count;
        for (int i = 0; i < count - 1; i++)
        {
            int realIndex = (i + nextIndex) % count;
            if (_weapons.TryGetValue(weapons[realIndex], out uiWeapon))
            {
                uiWeapon.SetUnactive();
                _unactiveList.AddChild(uiWeapon);
            }
        }
    }
/*
    private void HandleSwitch(PW_Weapon active, PW_Weapon nextHolster, int nextIndex, Godot.Collections.Array<PW_Weapon> weapons)
    {
        //_active.Initialize(active);
        //_holster.Initialize(nextHolster);
        _list.SetWeapons(weapons, nextIndex);
    }*/
}