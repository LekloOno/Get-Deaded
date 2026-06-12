using Godot;

public partial class VFX_VaultWind : Node
{
    [ExportCategory("Settings")]
    [Export] private ANIM_InOutTweenSetting _tweenSettings = null!;

    [ExportCategory("Setup")]
    [Export] private VFX_SpeedWindController _wind = null!;
    [Export] private PM_LedgeClimb _ledgeClimb = null!;

    private Tween? _intensityTween;

    public float Intensity
    {
        get => _wind.Intensity;
        set => _wind.Intensity = value;
    }

    public override void _Ready()
    {
        _wind.Intensity = 0;
        _ledgeClimb.SuperGlideStarted += OnSuperGlide;
    }

    private void OnSuperGlide()
    {
        _intensityTween?.Kill();

        _intensityTween = CreateTween();

        _tweenSettings.FadeIn.TweenProperty(_intensityTween, this);
        _tweenSettings.FadeOut.TweenProperty(_intensityTween, this);
    }
}