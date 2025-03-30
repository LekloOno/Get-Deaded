using System.Collections.Generic;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class UI_UnactiveWeaponsList : VBoxContainer
{
    [Export] private UI_WeaponHolder _template;
    private List<UI_WeaponHolder> _weaponHolders = [];

    public void InitializeWeapons(Array<PW_Weapon> weapons, int nextIndex)
    {
        foreach (Node child in GetChildren())
            RemoveChild(child);

        int count = weapons.Count;
        for (int i = 0; i < count - 1; i++)
        {
            int realIndex = (i + nextIndex) % count;
            AddWeapon(weapons[realIndex]);
        }
    }

    public void AddWeapon(PW_Weapon weapon)
    {
        UI_WeaponHolder weaponHolder = (UI_WeaponHolder) _template.Duplicate();
        weaponHolder.Initialize(weapon);
        _weaponHolders.Add(weaponHolder);
        AddChild(weaponHolder);
        MoveChild(weaponHolder, 0);
    }

    public void SetWeapons(Array<PW_Weapon> weapons, int nextIndex)
    {
        int count = weapons.Count;
        for (int i = 0; i < _weaponHolders.Count; i++)
        {
            int realIndex = (i + nextIndex) % count;
            _weaponHolders[i].Initialize(weapons[realIndex]);
        }
    }
}