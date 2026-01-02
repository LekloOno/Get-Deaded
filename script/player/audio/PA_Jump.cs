using System;
using Godot;

[GlobalClass]
public partial class PA_Jump : Node3D
{
    [ExportCategory("Settings")]
    [Export] private AUD2_Sound _wooshes;
    [Export] private AUD2_Sound _clothes;
    [Export] private AUD2_Sound _steps;
    [Export] private AUD2_Sound _wallJumpClothes;
    [Export] private AUD2_Sound _wallJumpWoosh;
    [Export] private AUD2_Sound _wallClimbClothes;
    [Export] private AUD2_Sound _wallClimbLow;
    [Export] private AUD2_Sound _wallClimbNoise;

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
        _wooshes.Play();
        _steps.Play();
        _wallJumpClothes.Play();
        _wallJumpWoosh.Play();
    }

    public void PlaySoundKick(object sender, EventArgs e)
    {
        _wooshes.Play();
        _steps.Play();
        
        _wallClimbNoise.Play();
        _wallClimbLow.Play();
    }

    public void PlaySoundHop(object sender, EventArgs e)
    {
        _wallClimbNoise.Play();
        _wallClimbClothes.Play();
        _wallClimbLow.Play();
    }

    public void PlaySoundWallClimb(object sender, EventArgs e)
    {
        _steps.Play();
        _wooshes.Play();
        _wallClimbLow.Play();
    }

    public void PlaySound(object sender, EventArgs e)
    {
        _wooshes.Play();
        _clothes.Play();
        _steps.Play();
    }
}