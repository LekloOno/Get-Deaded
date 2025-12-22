using System;
using Godot;

namespace Pew;

[GlobalClass]
public partial class PA_Jump : Node3D
{
    [ExportCategory("Settings")]
    [Export] private PA_Sound _wooshes;
    [Export] private PA_Sound _clothes;
    [Export] private PA_Sound _steps;
    [Export] private PA_Sound _wallJumpClothes;
    [Export] private PA_Sound _wallJumpWoosh;
    [Export] private PA_Sound _wallClimbClothes;
    [Export] private PA_Sound _wallClimbLow;
    [Export] private PA_Sound _wallClimbNoise;

    [ExportCategory("Setup")]
    [Export] private PM_Jump _jump;
    [Export] private PM_WallJump _wallJump;
    [Export] private PM_WallClimb _wallClimb;

    public override void _Ready()
    {
        _jump.OnStart += PlaySound;
        _wallJump.OnStart += PlaySoundWall;
        _wallClimb.OnStart += PlaySoundWallClimb;
        _wallClimb.OnKick += PlaySoundKick;
        _wallClimb.OnHopStart += PlaySoundHop;
    }

    public void PlaySoundWall(object sender, EventArgs e)
    {
        _wooshes.PlaySound();
        _steps.PlaySound();
        _wallJumpClothes.PlaySound();
        _wallJumpWoosh.PlaySound();
    }

    public void PlaySoundKick(object sender, EventArgs e)
    {
        _wooshes.PlaySound();
        _steps.PlaySound();
        
        _wallClimbNoise.PlaySound();
        _wallClimbLow.PlaySound();
    }

    public void PlaySoundHop(object sender, EventArgs e)
    {
        _wallClimbNoise.PlaySound();
        _wallClimbClothes.PlaySound();
        _wallClimbLow.PlaySound();
    }

    public void PlaySoundWallClimb(object sender, EventArgs e)
    {
        _steps.PlaySound();
        _wooshes.PlaySound();
        _wallClimbLow.PlaySound();
    }

    public void PlaySound(object sender, EventArgs e)
    {
        _wooshes.PlaySound();
        _clothes.PlaySound();
        _steps.PlaySound();
    }
}