using System;
using System.Linq.Expressions;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class PA_Slide : Node3D
{
    [Export] private PA_LayeredSound _slideIn;
    [Export] private PA_LayeredSound _slideHold;
    [Export] private Timer _noiseLoop;

    [ExportCategory("Setup")]
    [Export] private PM_Slide _slide;
    [Export] private PS_Grounded _groundState;

    public override void _Ready()
    {
        _noiseLoop.Timeout += PlayNoise;

        _slide.OnStart += StartPlaySound;
        _slide.OnStop += (o, e) => _noiseLoop.Stop();

        _groundState.OnLeaving += StopPlaySound;
        //_groundState.OnLanding += TryStartNoise;
    }

    private void StartPlaySound(object sender, EventArgs e)
    {
        //_noiseLoop.Start();
        if(_groundState.IsGrounded())
            _slideIn.PlayLayers();
    }

    private void StopPlaySound(object sender, EventArgs e)
    {
        //_noiseLoop.Stop();
        _slideIn.StopLayers();
    }

    private void TryStartNoise(object sender, EventArgs e)
    {
        if (_slide.IsActive)
            _noiseLoop.Start();
    }

    private void PlayNoise() => _slideHold.PlayLayers();
}