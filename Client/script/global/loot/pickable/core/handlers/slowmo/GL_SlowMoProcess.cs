using System;
using Godot;

public partial class GL_SlowMoProcess : Node
{
    [Export] private float _fadeInTime = 1f;
    [Export] private float _fadeOutTime = .5f;
    [Export] private float _initDelay = 0.8f;
    [Export] private AudioStreamPlayer? _sound;

    // Just a preventive guard for multiple slow mo process being picked up at once.
    // If another one is picked up, it takes the lead.
    public static GL_SlowMoProcess? Active {get; private set;} = null;
    
    private SceneTreeTimer? _fadeInTimer;
    private SceneTreeTimer? _fadeOutTimer;

    public Action<float>? FadeInStarted;
    public Action<float>? DurationStarted;

    private float _factor;
    private float _duration;
    private Tween? _fadeTween;

    public double CurrentFactor
    {
        get => Engine.TimeScale;
        set
        {
            Engine.TimeScale = value;
        }
    }

    public void InitData(GL_SlowMoData data)
    {
        _factor = data.Factor;
        _duration = data.Duration;
    }

    public override void _Ready()
    {
        if (Active != null)
            Active.Abort(false);

        Active = this;

        if (_sound != null)
            _sound.Play();
        else
            GD.PushWarning("Missing stream player for GL_SlowMoProcess.");

        _fadeInTimer = GetTree().CreateTimer(_fadeInTime, false, false);
        _fadeInTimer.Timeout += FadeIn;
    }

    private async void FadeIn()
    {
        if (_fadeInTimer != null)
            _fadeInTimer.Timeout -= FadeIn;

        _fadeTween?.Kill();
        _fadeTween = CreateTween();
        _fadeTween.TweenProperty(this, "CurrentFactor", _factor, _fadeInTime);
        FadeInStarted?.Invoke(_fadeInTime);

        await ToSignal(_fadeTween, "finished");


        _fadeOutTimer = GetTree().CreateTimer(_duration, false, false);
        _fadeOutTimer.Timeout += FadeOut;
        DurationStarted?.Invoke(_duration);
    }

    private async void FadeOut()
    {
        if (_fadeOutTimer != null)
            _fadeOutTimer.Timeout -= FadeOut;

        _fadeTween?.Kill();
        _fadeTween = CreateTween();
        _fadeTween.TweenProperty(this, "CurrentFactor", 1f, _fadeOutTime);

        await ToSignal(_fadeTween, "finished");

        
        Active = null;
        QueueFree();
    }

    public void Abort(bool resetTimeScale)
    {
        _fadeTween?.Kill();

        if (_fadeInTimer != null)
            _fadeInTimer.Timeout -= FadeIn;

        if (_fadeOutTimer != null)
            _fadeOutTimer.Timeout -= FadeOut;

        if (resetTimeScale)
            Engine.TimeScale = 1f;

        Active = null;
        QueueFree();
    }
}