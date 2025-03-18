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

    private StyleBoxFlat _hitStyle;
    private Tween opacityTween;

    public override void _Ready()
    {
        _weaponsHandler.Hit += HandleHit;
        _hitStyle = (StyleBoxFlat) _markerSticks.ElementAt(0).GetThemeStylebox("panel");
        Color mod = Modulate;
        mod.A = 0f;
        Modulate = mod;
    }

    public void HandleHit(object sender, ShotHitEventArgs e)
    {
        if (e.HurtBox == null || e.Target == null)
            return;
        
        Color mod = Modulate;
        mod.A = 1f;
        Modulate = mod;

        opacityTween?.Kill();

        if (e.HurtBox.BodyPart == GC_BodyPart.Head)
            _hitStyle.BgColor = _headShotColor;
        else
            _hitStyle.BgColor = _normalColor;
        
        opacityTween = CreateTween();
        opacityTween.TweenProperty(this, "modulate:a", 0.0f, _fadeTime);

    }
}