using System;
using Godot;

[GlobalClass]
public partial class PA_Jump : Node3D
{
    [ExportCategory("Settings")]
    [Export] private PA_Sound _wooshes;
    [Export] private PA_Sound _clothes;
    [Export] private PA_Sound _steps;
    [Export] private PA_Sound _wallJumpClothes;
    [Export] private PA_Sound _wallJumpWoosh;

    [ExportCategory("Setup")]
    [Export] private PM_Jump _jump;
    [Export] private PM_WallJump _wallJump;

    public override void _Ready()
    {
        _jump.OnStart += PlaySound;
        _wallJump.OnStart += PlaySoundWall;
    }

    public void PlaySoundWall(object sender, EventArgs e)
    {
        _wooshes.PlaySound();
        _steps.PlaySound();
        _wallJumpClothes.PlaySound();
        _wallJumpWoosh.PlaySound();
    }

    public void PlaySound(object sender, EventArgs e)
    {
        _wooshes.PlaySound();
        _clothes.PlaySound();
        _steps.PlaySound();
    }
}