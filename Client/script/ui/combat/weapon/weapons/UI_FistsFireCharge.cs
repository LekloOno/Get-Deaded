using Godot;

[GlobalClass]
public partial class UI_FistsFireCharge : TextureProgressBar
{
    [Export] private PW_FistsFire _fistFire = null!;

    [Export] private ANIM_InOutTweenSetting _opacityFadeSetting = null!;
    [Export] private ANIM_TweenSetting _activeColorTweenSetting = null!;
    [Export] private ANIM_TweenSetting _unactiveColorTweenSetting = null!;

    private ulong _lastShot;

    private Tween? _chargeTween;
    private Tween? _opacityTween;
    private Tween? _colorTween;

    public override void _Ready()
    {
        Visible = false;
        _fistFire.ChargeStarted += OnChargeStarted;
        _fistFire.ChargeCancelled += OnChargeCancelled;
        _fistFire.Shot += OnShot;
        _fistFire.Disabled += OnChargeCancelled;
    }

    private void OnShot(object? sender, int e)
    {
        _colorTween?.Kill();
        _colorTween = CreateTween();
        _unactiveColorTweenSetting.TweenProperty(_colorTween, this, propertyPath: "self_modulate");

        _lastShot = PHX_Time.ScaledTicksMsec;
        SetProcess(true);

        OnChargeCancelled();
    }

    public override void _ExitTree()
    {
        _opacityTween?.Kill();
        _chargeTween?.Kill();
        _colorTween?.Kill();

        Visible = false;
    }

    private void OnChargeCancelled()
    {
        _opacityTween?.Kill();
        _opacityTween = CreateTween();
        _opacityFadeSetting.FadeOut.TweenProperty(_opacityTween, this, 0f, "modulate:a");

        _opacityTween.Finished += () => { if (IsInsideTree()) Visible = false; };
    }

    public void OnChargeStarted(ulong time)
    {
        Visible = true;

        _opacityTween?.Kill();
        _opacityTween = CreateTween();
        _opacityFadeSetting.FadeOut.TweenProperty(_opacityTween, this, .8f, "modulate:a");

        Value = 0f;
        _chargeTween?.Kill();
        _chargeTween = CreateTween();
        _chargeTween.TweenProperty(this, "value", 1f, time / 1000f);
    }

    public override void _Process(double delta)
    {
        if (PHX_Time.ScaledTicksMsec - _lastShot >= _fistFire.FireRate)
        {
            _colorTween?.Kill();
            _colorTween = CreateTween();
            _activeColorTweenSetting.TweenProperty(_colorTween, this, propertyPath: "self_modulate");

            SetProcess(false);
        }
    }
}