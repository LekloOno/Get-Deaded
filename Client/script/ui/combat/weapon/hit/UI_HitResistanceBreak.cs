using Godot;

public partial class UI_HitResistanceBreak : Control
{
    [Export] private TextureRect _halo = null!;
    [Export] private float _staleTime = 1f;
    [Export] private ANIM_InOutTweenSetting _opacitySettings = null!;
    [Export] private ANIM_TweenSetting _scaleSetting = null!;

    private Tween? _opacityTween;
    private Tween? _scaleTween;

    public void Initialize()
    {
        Color mod = Modulate;
        mod.A = 0f;
        Modulate = mod;
        
        Scale = Vector2.Zero;

        _opacityTween = CreateTween();
        _scaleTween = CreateTween();

        _scaleSetting.TweenProperty(_scaleTween, this, null, "scale");
        
        _opacitySettings.FadeIn.TweenProperty(_opacityTween, this, 1f, "modulate:a");
        if (_staleTime > 0f)
            _opacityTween.TweenInterval(_staleTime);
        _opacitySettings.FadeOut.TweenProperty(_opacityTween, this, 0f, "modulate:a");

        _opacityTween.Finished += QueueFree;
    }
}