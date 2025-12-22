using Godot;
using System;

namespace Pew;

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
    private uint _successors = 0;
    private Vector2 _targetPos;
    private bool _isFading = false;

    public void Init(UI_KillSkullManager manager)
    {
        
        _manager = manager;
        // Manager connections
        _manager.PushSkull += Push;         // Push is unsubed by removing the node.
        _manager.PushSkull += OverflowFade; // Calling OverflowFade *eventually* calls Fade
        _manager.FadeTimer.Timeout += Fade; // Calling Fade unsub itself and OverflowFade

        // Sets initial opacity, scale and Position
        // Hide
        Color mod = Modulate;
        Color targetColor = mod;
        mod.A = 0f;
        Modulate = mod;
        // Center
        Position = - Size/2;
        // Size to zero
        Scale = Vector2.Zero;


        // Animation first step : Impact scale up & fade in
        _opacityTween = CreateTween();
        _opacityTween.TweenProperty(this, "modulate", targetColor, _fadeInTime);
        
        _scaleTween = CreateTween();
        _scaleTween
            .TweenProperty(this, "scale", new Vector2(_maxScale, _maxScale), _maxScaleTime)
            .SetTrans(_scaleInTrans);
        
        MaxScaleReached?.Invoke();

        // Animation step 2 : Scale down to normal scale, & offset to base side position.
        _targetPos = Position + _offset;

        _offsetTween = CreateTween();
        _offsetTween
            .TweenProperty(this, "position", _targetPos, _offsetTime)
            .SetTrans(_offsetTrans);

        _scaleTween
            .TweenProperty(this, "scale", new Vector2(_normalScale, _normalScale), _normalScaleTime)
            .SetTrans(_scaleOutTrans);
    }

    /// <summary>
    /// Translates itself for leaving space for the new pushed KillSkull.
    /// </summary>
    public void Push()
    {
        _targetPos += new Vector2(Size.X*_normalScale, 0);

        _offsetTween?.Kill();
        _offsetTween = CreateTween();
        _offsetTween
            .TweenProperty(this, "position", _targetPos, _offsetTime)
            .SetTrans(_offsetTrans);
    }


    // REMOVING STEPS
    // We can keep the pushing event handler until the node is completely removed,
    // as it can still be translated even if it has already started to fade out.
    // However, we want to do the fading operation only once, so it is a one shot event.

    /// <summary>
    /// Initially bounded to its _manager PushSkull Event.
    /// Checks if this KillSkull overflows the max chain size.
    /// If so, it calls Fade().
    /// </summary>
    public void OverflowFade()
    {
        if (_successors ++ <= _manager.MaxChainSize)
            return;

        Fade();
    }

    /// <summary>
    /// One-shot handler initially bounded to its _manager FadeTimer.Timeout Event.
    /// Tweens its opacity to 0 to fade it out, unsubscribe itself and PushOverflow from the Timeout Event,
    /// and bind Remove() to the end of the tween operation.
    /// </summary>
    public void Fade()
    {
        _manager.PushSkull -= OverflowFade;
        _manager.FadeTimer.Timeout -= Fade;
        _opacityTween = CreateTween();
        _opacityTween.TweenProperty(this, "modulate:a", .0f, _fadeOutTime);
        _opacityTween.Finished += Remove;
    }

    /// <summary>
    /// Eventually Bounded to _opacityTween.Finished Event.
    /// Unsubscribe Push event from its _manager PushSkull Event, and Free the node.
    /// </summary>
    public void Remove()
    {
        _manager.PushSkull -= Push;
        Removed?.Invoke();
        QueueFree();
    }
}