using System;
using Godot;
using Godot.Collections;

namespace Pew;

[GlobalClass]
public partial class PA_FootSteps : PA_Sound
{
    [ExportCategory("Settings")]
    [Export] private float _normalInterval = 0.35f;
    [Export] private float _normalPitchBase = 0.8f;
    [Export] private float _normalVolume = -40f;

    [Export] private float _sprintInterval = 0.3f;
    [Export] private float _sprintPitchBase = 1f;
    [Export] private float _sprintVolume = -25f;

    [ExportCategory("Setup")]
    [Export] private PI_Walk _walkInput;
    [Export] private PM_SurfaceState _groundSurfaceState;
    [Export] private PS_Grounded _groundState;
    [Export] private Timer _stepLoop;
    private float _currentInterval;
    
    public override void _Ready()
    {
        _stepLoop.Timeout += PlaySound;

        _walkInput.Stop += DirectStopPlay;
        _groundState.OnLeaving += StopPlay;
        _groundSurfaceState.Sprint.OnStop += StopPlay;
        _groundSurfaceState.Normal.OnStop += StopPlay;

        _walkInput.Start += TryInputPlay;
        _groundState.OnLanding += TryLandPlay;
        _groundSurfaceState.Sprint.OnStart += TryPlaySprint;
        _groundSurfaceState.Normal.OnStart += TryPlayNormal;
    }

    public void TryInputPlay(object sender, Vector2 axis)
    {
        // Grounded check is already done by TryPlayNormal/Sprint
        // So we just dispatch to the right listener.

        if (_groundSurfaceState.IsNormal())
            TryPlayNormal(sender, EventArgs.Empty);

        if (_groundSurfaceState.IsSprint())
            TryPlaySprint(sender, EventArgs.Empty);
    }

    public void TryLandPlay(object sender, EventArgs e)
    {
        // Just landed, no need to check for grounded state.

        if (_walkInput.IsStopped())
            return;

        if (_groundSurfaceState.IsNormal())
            StartPlayNormal();
        
        if (_groundSurfaceState.IsSprint())
            StartPlaySprint();
    }

    public void TryPlaySprint(object sender, EventArgs e)
    {
        if (!_groundState.IsGrounded())
            return;


        StartPlaySprint();
    }

    public void TryPlayNormal(object sender, EventArgs e)
    {
        if (!_groundState.IsGrounded() || _walkInput.IsStopped())
            return;

        StartPlayNormal();
    }

    private void StartPlayNormal()
    {
        _pitchBaseDelta = _normalPitchBase - 1f;
        VolumeDb = _normalVolume;
        _stepLoop.Start(_normalInterval);
    }
    private void StartPlaySprint()
    {
        _pitchBaseDelta = _sprintPitchBase - 1f;
        VolumeDb = _sprintVolume;
        _stepLoop.Start(_sprintInterval);
    }   

    public void StopPlay(object sender, EventArgs e) => _stepLoop.Stop();
    public void DirectStopPlay(object sender, Vector2 axis) => _stepLoop.Stop();
}