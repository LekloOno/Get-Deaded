using Godot;

public partial class UI_HitResistance : Control
{
    [Export] private TextureRect _body = null!;
    [Export] private TextureRect _halo = null!;
    [Export] private float _staleTime = 1f;
    [Export] private ANIM_InOutTweenSetting _opacitySettings = null!;
    [Export] private ANIM_TweenSetting _noResistOpacitySetting = null!;
    [Export] private PackedScene _breakIcon = null!;

    private Tween? _opacityTween;

    public void Initialize(HitEventArgs args)
    {
        Color mod = Modulate;
        mod.A = 0f;
        Modulate = mod;
        
        PlayHit(args);
    }

    public void PlayHit(HitEventArgs args)
    {   
        _opacityTween?.Kill();

        if (args.BrokeLayer)
        {
            UI_HitResistanceBreak breakIcon = _breakIcon.Instantiate<UI_HitResistanceBreak>();
            GetParent().AddChild(breakIcon);
            breakIcon.Position = Position;
            breakIcon.Initialize();

            Color mod = Modulate;
            mod.A = 0f;
            Modulate = mod;
            return;
        }

        bool resist = args.Resistance > 0.025f;

        _opacityTween = CreateTween();

        if (!resist)
        {
            _noResistOpacitySetting.TweenProperty(_opacityTween, this, 0f, "modulate:a");
            return;
        }

        _halo.Modulate = CONF_HealthColors.GetDamageColors(args.SenderLayer);
        
        float targetAlpha = args.Resistance;


        _opacitySettings.FadeIn.TweenProperty(_opacityTween, this, targetAlpha, "modulate:a");
        if (_staleTime > 0f)
            _opacityTween.TweenInterval(_staleTime);
        _opacitySettings.FadeOut.TweenProperty(_opacityTween, this, 0f, "modulate:a");
    }
}