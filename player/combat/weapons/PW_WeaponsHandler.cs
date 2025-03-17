//using System.Linq;
using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class PW_WeaponsHandler : Node
{
    [Export] private Camera3D _camera;
    [Export] private Node3D _sight;
    [Export] private PI_Weapons _weaponsInput;
    [Export] private Array<PW_Weapon> _weapons;
    private PW_Weapon _activeWeapon;
    public override void _Ready()
    {
        foreach(PW_Weapon weapon in _weapons)
            weapon.Initialize(_camera, _sight);

        if (_weapons.Count > 0)
            _activeWeapon = _weapons[0];

        _weaponsInput.OnStartPrimary += (o, e) => _activeWeapon.PrimaryDown();
        _weaponsInput.OnStopPrimary += (o, e) => _activeWeapon.PrimaryUp();
        _weaponsInput.OnStartSecondary += (o, e) => _activeWeapon.SecondaryDown();
        _weaponsInput.OnStopSecondary += (o, e) => _activeWeapon.SecondaryUp();
    }
}