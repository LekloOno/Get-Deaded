using System;
using System.Data;
using System.Linq.Expressions;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class PA_Slide : Node3D
{
    [Export] private PA_LayeredSound _slideIn;
    [Export] private PA_Looper _slideHold;

    [ExportCategory("Setup")]
    [Export] private PM_Slide _slide;
    [Export] private PS_Grounded _groundState;

    private EventHandler OnUpdate;

    private float _fadeStart;
    private float _startVolume;

    public override void _Ready()
    {
        _slideHold.VolumeDb = -80;

        _slide.OnStart += StartPlaySound;
        _slide.OnStop += (o, e) => _slideHold.StopLoop();
        _slide.OnSlowStop += (o, e) => _slideHold.StopLoop();

        _groundState.OnLeaving += StopPlaySound;
        _groundState.OnLanding += TryLandStartHold;
    }
    public override void _PhysicsProcess(double delta) => OnUpdate?.Invoke(this, EventArgs.Empty);

    private void StartPlaySound(object sender, EventArgs e)
    {
        if(_groundState.IsGrounded())
        {
            _slideIn.PlayLayers();
            _slideHold.StartLoop();
        }
    }

    private void StopPlaySound(object sender, EventArgs e)
    {
        _slideIn.StopLayers();
        _slideHold.StopLoop();
    }

    private void TryLandStartHold(object sender, EventArgs e)
    {
        if (_slide.IsActive)
            _slideHold.StartLoop();
    }
}