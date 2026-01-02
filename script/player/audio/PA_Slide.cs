using System;
using Godot;

[GlobalClass]
public partial class PA_Slide : Node3D
{
    [Export] private AUD2_Sound _slideIn;
    [Export] private AUD2_Sound _slideHold;

    [ExportCategory("Setup")]
    [Export] private PM_Slide _slide;
    [Export] private PS_Grounded _groundState;

    private EventHandler OnUpdate;

    private float _fadeStart;
    private float _startVolume;

    public override void _Ready()
    {
        //_slideHold.VolumeDb = -80;

        _slide.OnStart += StartPlaySound;
        _slide.OnStop += (o, e) => _slideHold.Stop();
        _slide.OnSlowStop += (o, e) => _slideHold.Stop();

        _groundState.OnLeaving += StopPlaySound;
        _groundState.OnLanding += TryLandStartHold;
    }
    public override void _PhysicsProcess(double delta) => OnUpdate?.Invoke(this, EventArgs.Empty);

    private void StartPlaySound(object sender, EventArgs e)
    {
        if(_groundState.IsGrounded())
        {
            _slideIn.Play();
            _slideHold.Play();
        }
    }

    private void StopPlaySound(object sender, EventArgs e)
    {
        _slideIn.Stop();
        _slideHold.Stop();
    }

    private void TryLandStartHold(object sender, EventArgs e)
    {
        if (_slide.IsActive)
            _slideHold.Play();
    }
}