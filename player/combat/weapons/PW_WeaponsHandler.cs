//using System.Linq;
using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class PW_WeaponsHandler : Node
{
    [Export] private Node3D _sight;
    [Export] private PI_Weapons _weaponsInput;
    [Export] private Array<PW_Weapon> _weapons;
    private PW_Weapon _activeWeapon;
    public override void _Ready()
    {
        if (_weapons.Count > 0)
            _activeWeapon = _weapons[0];

        _weaponsInput.OnStartPrimary += Primary;
    }

    public void Primary(object sender, EventArgs e)
    {
        _activeWeapon?.Shoot(_sight);
    }
}