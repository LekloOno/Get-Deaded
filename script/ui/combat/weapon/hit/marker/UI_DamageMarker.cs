using System.Linq;
using Godot;
using Godot.Collections;

namespace Pew;

[GlobalClass]
public partial class UI_DamageMarker : Control
{
    [Export] private Array<Panel> _markerSticks;
    [Export] private float _fadeTime = .18f;
    [Export] private float _expandTime = .1f;
    [Export] private float _baseStartOffset = 12f;
    [Export] private float _basePeakOffset = 15f;
    [Export] private Vector2 _baseSize = new(16f, 2f);

    // Higher values means higher damages are required to get the maximum size.
    // It is the dispersion of the tanh() function used to convert the damage to a size.
    [Export] private float _sizeDispersion = 300f;
    // The maximum size coeficient, x times bigger than _baseSize.
    [Export] private float _sizeMaxCoef = 5f; // The final coeficient (-1) could be computed on ready
    
    // Same as for the dispersion but for the peak offset
    [Export] private float _offsetDispersion = 20f;
    [Export] private float _offsetMaxCoef = 2.5f;

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
        Offset = _baseStartOffset;
    }

    public void StartAnim(HitEventArgs e)
    {
        Color mod = Modulate;
        mod.A = 1f;
        Modulate = mod;

        opacityTween?.Kill();
        offsetTween?.Kill();

        float sizeRatio = Mathf.Tanh(e.TotalDamage/_sizeDispersion)*(_sizeMaxCoef - 1f) + 1f;
        SticksSize = _baseSize * sizeRatio;

        if (!e.OverrideBodyPart && e.HurtBox.BodyPart == GC_BodyPart.Head)
            _hitStyle.BgColor = CONF_HitColors.Colors.Critical;
        else
            _hitStyle.BgColor = CONF_HitColors.Colors.Normal;
        
        opacityTween = CreateTween();
        opacityTween.TweenProperty(this, "modulate:a", 0.0f, _fadeTime);

        offsetTween = CreateTween();
        float scaledDamage = Mathf.Tanh(e.TotalDamage/_offsetDispersion) * (_offsetMaxCoef-1f) + 1;

        offsetTween.TweenProperty(this, "Offset", _basePeakOffset * scaledDamage, _expandTime).SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Sine);
        offsetTween.TweenProperty(this, "Offset", _baseStartOffset, _expandTime);
    }
}