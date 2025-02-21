using System;
using Godot;

[GlobalClass]
public partial class PA_Jump : Node3D
{
    [ExportCategory("Settings")]
    [Export] private PA_Sound _wooshes;
    [Export] private PA_Sound _clothes;
    [Export] private PA_Sound _steps;
    
    [ExportCategory("Setup")]
    [Export] private PM_Jump _jump;

    public override void _Ready()
    {
        _jump.OnStart += PlaySound;
    }

    public void PlaySound(object sender, EventArgs e)
    {
        _wooshes.PlaySound();
        _clothes.PlaySound();
        _steps.PlaySound();
    }
}