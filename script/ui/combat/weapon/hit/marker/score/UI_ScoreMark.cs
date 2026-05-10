using Godot;

[GlobalClass]
public partial class UI_ScoreMark : Label
{
    [Export] private float _maxScale;
    [Export] private float _maxScaleTime;
    [Export] private Tween.TransitionType _scaleInTrans;
    [Export] private float _normalScale;
    [Export] private float _normalScaleTime;
    [Export] private Tween.TransitionType _scaleOutTrans;
    [Export] private Vector2 _offset;   // Maybe use a target point instead so it can scale with ui
    [Export] private float _offsetTime;
    [Export] private Tween.TransitionType _offsetTrans;
    [Export] private float _fadeOutTime;
    [Export] private float _fadeInTime;

    private Tween _opacityTween;
    private Tween _scaleTween;
    private Tween _offsetTween;
    private UI_ScoreMarkManager _manager;
    private uint _successors = 0;
    private Vector2 _targetPos;
    private bool _isFading = false;

    public override void _Ready()
    {
        base._Ready();
    }


    public void Init()
    {
        // Sets initial opacity, scale and Position
        // Hide
        Color mod = Modulate;
        mod.A = 0f;
        Modulate = mod;
        // Center
        Position = - Size/2;
        // Size to zero
        Scale = Vector2.Zero;


        // Animation first step : Impact scale up & fade in
        _opacityTween = CreateTween();
        _opacityTween.TweenProperty(this, "modulate:a", 1f, _fadeInTime);

        _targetPos = Position + _offset;

        _offsetTween = CreateTween();
        _offsetTween
            .TweenProperty(this, "position", _targetPos, _offsetTime)
            .SetTrans(_offsetTrans);
        
        Accumulate();
    }

    /// <summary>
    /// Translates itself for leaving space for the new pushed KillSkull.
    /// </summary>
    public void Accumulate()
    {   
        _scaleTween = CreateTween();
        _scaleTween
            .TweenProperty(this, "scale", new Vector2(_maxScale, _maxScale), _maxScaleTime)
            .SetTrans(_scaleInTrans);

        // Animation step 2 : Scale down to normal scale, & offset to base side position.

        _scaleTween
            .TweenProperty(this, "scale", new Vector2(_normalScale, _normalScale), _normalScaleTime)
            .SetTrans(_scaleOutTrans);
    }


    // REMOVING STEPS
    // We can keep the pushing event handler until the node is completely removed,
    // as it can still be translated even if it has already started to fade out.
    // However, we want to do the fading operation only once, so it is a one shot event.

    /// <summary>
    /// One-shot handler initially bounded to its _manager FadeTimer.Timeout Event.
    /// Tweens its opacity to 0 to fade it out, unsubscribe itself and PushOverflow from the Timeout Event,
    /// and bind Remove() to the end of the tween operation.
    /// </summary>
    public void Fade()
    {
        _opacityTween = CreateTween();
        _opacityTween.TweenProperty(this, "modulate:a", .0f, _fadeOutTime);
    }
}