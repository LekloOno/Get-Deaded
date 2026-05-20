using Godot;

public partial class GL_SlowMoProcess : Node
{
    [Export] private float _fadeInTime = 1f;
    [Export] private float _fadeOutTime = .5f;
    [Export] private float _initDelay = 0.8f;
    [Export] private AudioStreamPlayer? _sound;

    // Just a preventive guard for multiple slow mo process being picked up at once.
    // If another one is picked up, it takes the lead.
    private static GL_SlowMoProcess? _active = null;
    
    private SceneTreeTimer? _fadeInTimer;
    private SceneTreeTimer? _fadeOutTimer;

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
        if (_active != null)
            _active.Abort(false);

        _active = this;

        if (_sound != null)
            _sound.Play();
        else
            GD.PushWarning("Missing stream player for GL_SlowMoProcess.");

        _fadeInTimer = GetTree().CreateTimer(_initDelay, false, false);
        _fadeInTimer.Timeout += FadeIn;
    }

    private async void FadeIn()
    {        
        if (_fadeInTimer != null)
            _fadeInTimer.Timeout -= FadeIn;

        _fadeTween?.Kill();
        _fadeTween = CreateTween();
        _fadeTween.TweenProperty(this, "CurrentFactor", _factor, _fadeInTime);
        
        await ToSignal(_fadeTween, "finished");

        _fadeOutTimer = GetTree().CreateTimer(_duration, false, false);
        _fadeOutTimer.Timeout += FadeOut;
    }

    private async void FadeOut()
    {
        if (_fadeOutTimer != null)
            _fadeOutTimer.Timeout -= FadeOut;

        _fadeTween?.Kill();
        _fadeTween = CreateTween();
        _fadeTween.TweenProperty(this, "CurrentFactor", 1f, _fadeOutTime);

        await ToSignal(_fadeTween, "finished");

        
        _active = null;
        QueueFree();
    }

    private void Abort(bool resetTimeScale)
    {
        _fadeTween?.Kill();

        if (_fadeInTimer != null)
        {
            _fadeInTimer.Timeout -= FadeIn;
            _fadeInTimer = null;
        }

        if (_fadeOutTimer != null)
        {
            _fadeOutTimer.Timeout -= FadeOut;
            _fadeOutTimer = null;
        }

        if (resetTimeScale)
            Engine.TimeScale = 1f;

        _active = null;
        QueueFree();
    }
}