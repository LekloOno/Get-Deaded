using System.Linq;
using Godot;
using Godot.Collections;

namespace Pew;

[GlobalClass]
public partial class UI_KillMarker : Control
{
    [Export] private Array<Panel> _markerSticks;
    [Export] private float _fadeTime;
    [Export] private float _fadeBackTime;
    [Export] private float _baseStartOffset;
    [Export] private float _basePeakOffset;

    private Tween opacityTween;
    private Tween offsetTween;

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
        Color mod = CONF_HitColors.Colors.Critical;
        mod.A = 0f;
        Modulate = mod;
        Offset = _baseStartOffset;
    }

    public void StartAnim()
    {
        Color mod = CONF_HitColors.Colors.Critical;
        mod.A = 1f;
        Modulate = mod;

        opacityTween?.Kill();
        offsetTween?.Kill();

        opacityTween = CreateTween();
        opacityTween.TweenProperty(this, "modulate:a", 0.0f, _fadeTime);

        offsetTween = CreateTween();
        offsetTween.TweenProperty(this, "Offset", _basePeakOffset, _fadeTime).SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Cubic);
        offsetTween.TweenProperty(this, "Offset", _baseStartOffset, _fadeBackTime);
    }
}