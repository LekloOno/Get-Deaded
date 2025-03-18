using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class UI_HitMarker : Control
{
    [Export] private Array<Panel> _markerSticks;
    [Export] private PW_WeaponsHandler _weaponsHandler;
    [Export] private Color _headShotColor;
    [Export] private Color _normalColor;
    [Export] private float _fadeTime;
    [Export] private float _expandTime;
    [Export] private float _baseStartOffset = 12f;
    [Export] private float _basePeakOffset = 15f;
    
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

    private StyleBoxFlat _hitStyle;
    private Tween opacityTween;
    private Tween offsetTween;

    public override void _Ready()
    {
        _weaponsHandler.Hit += HandleHit;
        _hitStyle = (StyleBoxFlat) _markerSticks.ElementAt(0).GetThemeStylebox("panel");
        Color mod = Modulate;
        mod.A = 0f;
        Modulate = mod;
        Offset = _baseStartOffset;
    }

    public void HandleHit(object sender, ShotHitEventArgs e)
    {
        if (e.HurtBox == null || e.Target == null)
            return;
        
        Color mod = Modulate;
        mod.A = 1f;
        Modulate = mod;

        opacityTween?.Kill();
        offsetTween?.Kill();

        if (e.HurtBox.BodyPart == GC_BodyPart.Head)
            _hitStyle.BgColor = _headShotColor;
        else
            _hitStyle.BgColor = _normalColor;
        
        opacityTween = CreateTween();
        opacityTween.TweenProperty(this, "modulate:a", 0.0f, _fadeTime);

        offsetTween = CreateTween();
        float scaledDamage = Mathf.Tanh(e.Damage/20f) * 1.5f + 1;

        offsetTween.TweenProperty(this, "Offset", _basePeakOffset * scaledDamage, _expandTime);
        offsetTween.TweenProperty(this, "Offset", _baseStartOffset, _expandTime);
    }
}