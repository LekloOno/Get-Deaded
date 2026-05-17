using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class UIW_Loadout : BoxContainer
{
    [Export] private PW_WeaponsHandler _weaponsHandler;
    [Export] private UIW_Weapon _weaponTemplate;
    [Export] private Container _holster;
    [Export] private Container _active;
    [Export] private Container _next;
    [Export] private Container _nextHolder;
    [Export] private Container _unactiveList;

    private Dictionary<PW_Weapon, UIW_Weapon> _weapons = [];
    public override void _Ready()
    {
        _weaponsHandler.SwitchStarted += Update;
        _weaponsHandler.GotInitialized += Initialize;

        _weaponsHandler.InitData(out PW_Weapon active, out PW_Weapon nextHolster, out int nextIndex, out Godot.Collections.Array<PW_Weapon> weapons);
        if (active == null)
            return;

        Initialize(active, nextHolster, nextIndex, weapons);
    }

    private void Initialize(PW_Weapon active, PW_Weapon nextHolster, int nextIndex, Godot.Collections.Array<PW_Weapon> weapons)
    {
        if (_weaponTemplate.GetParent() is Node parent)
            parent.RemoveChild(_weaponTemplate);

        foreach (UIW_Weapon weapon in _weapons.Values)
            weapon.QueueFree();

        _weapons.Clear();

        InitializeWeapon(active);
        InitializeWeapon(nextHolster);

        int count = weapons.Count;

        if (count > 1)
        {
            _nextHolder.Show();
            for (int i = 0; i < count - 1; i++)
            {
                int realIndex = (i + nextIndex) % count;
                InitializeWeapon(weapons[realIndex]);
            }
        } else
            _nextHolder.Hide();

        // OPTIMIZE ME
        // Initialization could be more efficient by avoiding one dictionnary lookup, since we already know the uiw elements.
        Update(active, nextHolster, nextIndex, weapons);
    }

    private void InitializeWeapon(PW_Weapon weapon)
    {
        UIW_Weapon uiWeapon = (UIW_Weapon) _weaponTemplate.Duplicate();
        uiWeapon.Initialize(weapon);
        _weapons.Add(weapon, uiWeapon);
    }

    private void Update(PW_Weapon active, PW_Weapon nextHolster, int nextIndex, Godot.Collections.Array<PW_Weapon> weapons)
    {
        UpdateWeapon(active, _active, true);
        UpdateWeapon(nextHolster, _holster, false);

        bool hasNext = weapons.Count > 1;
        _nextHolder.Visible = hasNext;
        
        if (!hasNext)
            return;

        UpdateWeapon(weapons[nextIndex], _next, false);

        int count = weapons.Count;
        for (int i = 1; i < count - 1; i++)
        {
            int realIndex = (i + nextIndex) % count;
            UpdateWeapon(weapons[realIndex], _unactiveList, false);
        }
    }

    private void UpdateWeapon(PW_Weapon weapon, Node targetParent, bool active)
    {
        if (!_weapons.TryGetValue(weapon, out UIW_Weapon uiWeapon))
            return;

        uiWeapon.Active = active;

        if (uiWeapon.GetParent() is Node parent)
        {
            if (parent == targetParent)
                return;
            parent.RemoveChild(uiWeapon);
        }

        targetParent.AddChild(uiWeapon);
    }
}