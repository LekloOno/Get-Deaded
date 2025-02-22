using System;
using System.Data;
using System.Linq.Expressions;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class PA_Slide : Node3D
{
    [Export] private PA_LayeredSound _slideIn;
    [Export] private AudioStreamPlayer2D _slideHold;
    [Export] private float _holdVolume;
    [Export] private float _holdFadeInTime;
    [Export] private float _holdFadeOutTime;

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
        _slide.OnStop += (o, e) => StopHold();

        _groundState.OnLeaving += StopPlaySound;
        _groundState.OnLanding += TryLandStartHold;
    }
    public override void _PhysicsProcess(double delta) => OnUpdate?.Invoke(this, EventArgs.Empty);

    private void StartPlaySound(object sender, EventArgs e)
    {
        if(_groundState.IsGrounded())
        {
            _slideIn.PlayLayers();
            StartHold();
        }
    }

    private void StopPlaySound(object sender, EventArgs e)
    {
        _slideIn.StopLayers();
        StopHold();
    }

    private void TryLandStartHold(object sender, EventArgs e)
    {
        if (_slide.IsActive)
            StartHold();
    }

    private void InitFade()
    {
        _fadeStart = Time.GetTicksMsec();
        _startVolume = _slideHold.VolumeDb;
        SetPhysicsProcess(true);
    }

    private void StartHold()
    {
        GD.Print("start");
        InitFade();
        OnUpdate += FadeIn;
    }

    private void StopHold()
    {
        GD.Print("stop");
        InitFade();
        OnUpdate += FadeOut;
    }


    private void Fade(EventHandler handler, float currentFadeTime, float currentTargetVolume)
    {
        float elapsed = (Time.GetTicksMsec() - _fadeStart)/1000f;

        if (elapsed >= currentFadeTime)
        {
            _slideHold.VolumeDb = currentTargetVolume;
            OnUpdate -= handler;
            SetPhysicsProcess(false);
            return;
        }

        float elapsedScaled = elapsed/currentFadeTime;
        float lerped = PHX_Math.LerpDB(_startVolume, currentTargetVolume, elapsedScaled);
        
        _slideHold.VolumeDb = lerped;
    }

    private void FadeOut(object sender, EventArgs e) => Fade(FadeOut, _holdFadeOutTime, -80);
    private void FadeIn(object sender, EventArgs e) => Fade(FadeIn, _holdFadeInTime, _holdVolume);
}