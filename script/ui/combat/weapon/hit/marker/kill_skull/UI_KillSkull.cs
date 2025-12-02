using Godot;
using System;

[GlobalClass]
public partial class UI_KillSkull : TextureRect
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

    public Action MaxScaleReached;
    public Action Removed;
    private Tween _opacityTween;
    private Tween _scaleTween;
    private Tween _offsetTween;
    private UI_KillSkullManager _manager;

    public void Init(UI_KillSkullManager manager)
    {
        
        _manager = manager;
        _manager.FadeTimer.Timeout += Fade;

        Color mod = Modulate;
        Color targetColor = mod;
        mod.A = 0f;
        Modulate = mod;

        Position = - Size/2;

        Scale = Vector2.Zero;

        _opacityTween = CreateTween();
        _opacityTween.TweenProperty(this, "modulate", targetColor, _fadeInTime);
        
        _scaleTween = CreateTween();
        _scaleTween
            .TweenProperty(this, "scale", new Vector2(_maxScale, _maxScale), _maxScaleTime)
            .SetTrans(_scaleInTrans);
        
        Vector2 pos = Position;

        _offsetTween = CreateTween();
        MaxScaleReached?.Invoke();
        manager.PushSkull += Push;

        _offsetTween
            .TweenProperty(this, "position", pos + _offset, _offsetTime)
            .SetTrans(_offsetTrans);

        _scaleTween
            .TweenProperty(this, "scale", new Vector2(_normalScale, _normalScale), _normalScaleTime)
            .SetTrans(_scaleOutTrans);
    }

    public void Push()
    {
        Vector2 pos = Position;
        
        _offsetTween = CreateTween();
        _offsetTween
            .TweenProperty(this, "position", pos + new Vector2(Size.X*Scale.X, 0), _offsetTime)
            .SetTrans(_offsetTrans);
    }

    public void Fade()
    {
        _opacityTween = CreateTween();
        _opacityTween.TweenProperty(this, "modulate:a", .0f, _fadeOutTime);
        _opacityTween.Finished += Remove;
    }

    public void Remove()
    {
        _manager.PushSkull -= Push;
        _manager.FadeTimer.Timeout -= Fade;
        Removed?.Invoke();
        QueueFree();
    }
}