using System;
using Godot;

[GlobalClass]
public partial class CNT_DashWeapon : Node
{
    [Export] private PM_Dash _dash;
    [Export] private PW_WeaponsHandler _weaponsHandler;
    private bool _buffered = false;
    public override void _Ready()
    {
        _dash.OnStart += DisableWeapons;
        _dash.OnStop += EnableWeapons;
    }

    private void EnableWeapons(object sender, EventArgs e) =>
        _weaponsHandler.EnableFire();

    private void DisableWeapons(object sender, EventArgs e) =>
        _weaponsHandler.DisableFire();
    
    
}