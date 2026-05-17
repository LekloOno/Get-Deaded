using Godot;

[GlobalClass]
public partial class UI_HitAura : Control
{
    [Export] private GC_HealthManager _healthManager;
    [Export] private StyleBoxFlat _vignetteStyle;
    [Export] private MATH_FloatCurveSampler _damageToBorderSampler;
    [Export] private Tween.TransitionType _flashInTrans;
    [Export] private float _flashInTime;
    [Export] private Tween.TransitionType _flashOutTrans;
    [Export] private float _flashOutTime;

    
    private Tween _flashTween;


    private int _borderRadius;
    public int BorderRadius
    {
        get => _borderRadius;
        set
        {
            _borderRadius = value;
            _vignetteStyle.BorderWidthBottom = value;
            _vignetteStyle.BorderWidthTop = value;
            _vignetteStyle.BorderWidthLeft = value;
            _vignetteStyle.BorderWidthRight = value;
        }
    } 
    private SceneTreeTimer _buffTimer;
    private double _timeLeft;
    private double _pulseTime;
    private float _accumulatedDamage;


    public override void _Ready()
    {
        _healthManager.OnDamage += ReceiveDamage;
        BorderRadius = 0;
    }

    private void ReceiveDamage(GC_Health senderLayer, DamageEventArgs e)
    {
        _vignetteStyle.BorderColor = CONF_HealthColors.GetDamageColors(senderLayer);

        if (_flashTween == null || !_flashTween.IsRunning())
            _accumulatedDamage = e.Amount;
        else
            _accumulatedDamage += e.Amount;

        int radius = (int) _damageToBorderSampler.Sample(_accumulatedDamage);

        _flashTween?.Kill();
        _flashTween = CreateTween();
        _flashTween
            .TweenProperty(this, "BorderRadius", radius, _flashInTime)
            .SetTrans(_flashInTrans)
            .SetEase(Tween.EaseType.Out);

        _flashTween = CreateTween();
        _flashTween
            .TweenProperty(this, "BorderRadius", 0, _flashOutTime)
            .SetTrans(_flashOutTrans);
    }
}