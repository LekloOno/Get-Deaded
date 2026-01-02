using System;
using Godot;

[GlobalClass]
public partial class PA_FootSteps : Node3D
{
    [Export] private AUD_Sound _normalSound;
    [Export] private AUD_Sound _sprintSound;

    [ExportCategory("Settings")]
    [Export] private float _normalInterval = 0.35f;
    [Export] private float _sprintInterval = 0.3f;

    [ExportCategory("Setup")]
    [Export] private PI_Walk _walkInput;
    [Export] private PM_SurfaceState _groundSurfaceState;
    [Export] private PS_Grounded _groundState;
    [Export] private Timer _stepLoop;
    private float _currentInterval;
    private AUD_Sound _currentSound;
    
    public override void _Ready()
    {
        _stepLoop.Timeout += Play;

        _walkInput.Stop += DirectStopPlay;
        _groundState.OnLeaving += StopPlay;
        _groundSurfaceState.Sprint.OnStop += StopPlay;
        _groundSurfaceState.Normal.OnStop += StopPlay;

        _walkInput.Start += TryInputPlay;
        _groundState.OnLanding += TryLandPlay;
        _groundSurfaceState.Sprint.OnStart += TryPlaySprint;
        _groundSurfaceState.Normal.OnStart += TryPlayNormal;
    }

    private void Play() => _currentSound.Play();

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
        _currentSound = _normalSound;
        _stepLoop.Start(_normalInterval);
    }
    private void StartPlaySprint()
    {
        _currentSound = _sprintSound;
        _stepLoop.Start(_sprintInterval);
    }   

    public void StopPlay(object sender, EventArgs e) => _stepLoop.Stop();
    public void DirectStopPlay(object sender, Vector2 axis) => _stepLoop.Stop();
}