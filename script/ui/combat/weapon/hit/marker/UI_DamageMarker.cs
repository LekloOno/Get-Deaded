using System;
using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class UI_DamageMarker : Control
{
    [Export] private Array<Panel> _markerSticks;
    [Export] private float _fadeTime = .3f;
    [Export] private float _fadeDelay = 0.4f;
    [Export] private float _expandTime = .01f;
    [Export] private float _tightenTime = .5f;
    [Export] private float _baseTightOffset = 15f;
    [Export] private float _basePeakOffset = 80f;
    [Export] private Vector2 _baseSize = new(10f, 2f);
    /// <summary>
    /// The 
    /// </summary>
    [Export] private Vector2 _maxDmgSize = new(30f, 5f);
    /// <summary>
    /// Higher values means higher damages are required to get the maximum size. <br/>
    /// It is the dispersion of the tanh() function used to convert the damage to a size. <br/>
    /// Concretely, for this exact amount of damage, the size will be interpolated around 76% between base and peak size. 
    /// </summary>
    [Export] private float _sizeDispersion = 300f;
    /// <summary>
    /// Higher values means higher damages are required to get the maximum offset. <br/>
    /// It is the dispersion of the tanh() function used to convert the damage to an offset. <br/>
    /// Concretely, for this exact amount of damage, the size will be interpolated around 76% between base and peak offset. 
    /// </summary>
    [Export] private float _offsetDispersion = 300f;
    /// <summary>
    /// The peak offset maximum damage coef. That is, how much damages can multiply the markers peak offset at most. 
    /// </summary>
    [Export] private float _offsetMaxDmgCoef = 2.5f;
    [Export] private Tween.TransitionType _offsetExpTransition = Tween.TransitionType.Back;
    [Export] private Tween.TransitionType _offsetTightTransition = Tween.TransitionType.Expo;
    [Export] private Tween.EaseType _offsetExpEase = Tween.EaseType.InOut;
    [Export] private Tween.EaseType _offsetTightEase = Tween.EaseType.Out;
    /// <summary>
    /// When multiple consecutive hits occur, the hit markers offset starts shrinking until reaching its tight offset at this much damage.
    /// </summary>
    [Export] private float _maxChainedDamage = 100f;
    /// <summary>
    /// Hit's damages are stacked into a damage chain if there's less than this amount of time between them.
    /// </summary>
    [Export] private ulong _chainLifetime = 150;
    /// <summary>
    /// The minimum peak offset the stacked damage makes the offset shrink to.
    /// </summary>
    [Export] private float _minPeakOffset = 17f;
    /// <summary>
    /// The minimum offset delta between peak and tight offset. This would eventually make the actual start offset tighter than _baseTightOffset.
    /// </summary>
    [Export] private float _minOffsetDelta = 5f;
    [Export] private ANIM_Vec2TraumaLayer _shakeLayer;
    [Export] private float _trauma = 1f;
    [Export] private float _shakeIntensity = 0.05f;
    [Export] private float _shakeMaxDmgCoef = 500f;
    [Export] private float _shakeDispersion = 100f;
    private float _shakeDmgCoef = 1f;

    private float _chainedDamage = 0f;
    private ulong _lastHit = 0;

    private StyleBoxFlat _hitStyle;
    private Tween opacityTween;
    private Tween offsetTween;
    private Tween sizeTween;

    private Vector2 _sticksSize;
    public Vector2 SticksSize
    {
        get => _sticksSize;
        set {
            Vector2 scale = value;
            Vector2 position = _markerSticks.ElementAt(0).Position;
            position.Y = -scale.Y/2;
            foreach (Panel stick in _markerSticks)
            {
                stick.Size = scale;
                stick.Position = position;
            }
            
            _sticksSize = value;
        }
    }

    private float _offset;
    public float Offset
    {
        get => _offset;
        set {
            Vector2 position = _markerSticks.ElementAt(0).Position;
            position.X = value;
            foreach (Panel stick in _markerSticks)
                stick.Position = position;
            
            _offset = value;
        }
    }

    public override void _Ready()
    {
        _hitStyle = (StyleBoxFlat) _markerSticks.ElementAt(0).GetThemeStylebox("panel");
        Color mod = Modulate;
        mod.A = 0f;
        Modulate = mod;
        Offset = _baseTightOffset;
    }

    private double _time = 0;

    public override void _Process(double delta)
    {
        _time += delta;
        Position = _shakeLayer.GetShakeAngleIntensity((float) delta, (float) _time) * _shakeIntensity * _shakeDmgCoef;
    }

    public void StartAnim(HitEventArgs e)
    {
        SetSize(e);
        SetColor(e); 
        AnimOpacity(e);
        AnimOffset(e);
        Shake(e);
    }

    private void Shake(HitEventArgs e)
    {
        _shakeDmgCoef = Mathf.Tanh(e.TotalDamage/_shakeDispersion) * (_shakeMaxDmgCoef - 1f) + 1f;
        _shakeLayer.AddTrauma(_trauma);
    }

    private void AnimOffset(HitEventArgs e)
    {
        offsetTween?.Kill();
        
        float offsetDmgCoef = Mathf.Tanh(e.TotalDamage/_offsetDispersion) * (_offsetMaxDmgCoef-1f) + 1;

        DamageChainOffset(e, out float peakOffset, out float tightOffset);

        offsetTween = CreateTween();
        offsetTween
            .TweenProperty(this, "Offset", peakOffset * offsetDmgCoef, _expandTime)
            .SetTrans(_offsetExpTransition)
            .SetEase(_offsetExpEase);

        offsetTween
            .TweenProperty(this, "Offset", tightOffset, _tightenTime)
            .SetTrans(_offsetTightTransition)
            .SetEase(_offsetTightEase);
    }

    private void DamageChainOffset(HitEventArgs e, out float peakOffset, out float tightOffset)
    {
        ulong now = PHX_Time.ScaledTicksMsec;
        if (now - _lastHit < _chainLifetime)
        {
            _chainedDamage += e.TotalDamage;
            peakOffset = Mathf.Lerp(_basePeakOffset, _minPeakOffset, Mathf.Min(_chainedDamage/_maxChainedDamage, 1));
            tightOffset = Mathf.Min(_baseTightOffset, peakOffset - _minOffsetDelta);
            Offset = Mathf.Lerp(tightOffset, peakOffset, 0.5f);
        }
        else
        {
            _chainedDamage = 0;
            peakOffset = _basePeakOffset;
            tightOffset = _baseTightOffset;   
        }

        _lastHit = now;
    } 

    private void AnimOpacity(HitEventArgs e)
    {
        opacityTween?.Kill();

        Color mod = Modulate;
        mod.A = 1f;
        Modulate = mod;

        opacityTween = CreateTween();
        opacityTween.TweenInterval(_fadeDelay);
        opacityTween.TweenProperty(this, "modulate:a", 0.0f, _fadeTime);
    }

    private void SetSize(HitEventArgs e)
    {
        float sizeRatio = Mathf.Tanh(e.TotalDamage/_sizeDispersion);
        SticksSize = _baseSize.Lerp(_maxDmgSize, sizeRatio);
    }

    private void SetColor(HitEventArgs e)
    {
        if (!e.OverrideBodyPart && e.HurtBox.BodyPart == GC_BodyPart.Head)
            _hitStyle.BgColor = CONF_HitColors.Colors.Critical;
        else
            _hitStyle.BgColor = CONF_HitColors.Colors.Normal;
    }
}