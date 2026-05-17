using System;
using Godot;

[GlobalClass]
public partial class UI_OmniCharge : TextureProgressBar
{
    [Export] private PM_OmniCharge _charge;
    [Export] private float _showTime = 0.1f;
    [Export] private float _hideTime = 0.5f;
    [Export] private float _blinkInTime;
    [Export] private float _blinkOutTime;
    [Export] private Color _blinkColor;

    /// <summary>
    /// Used to interpolate the charge in frame rate.
    /// Start is the previous buffered charge, target is the value we interpolate to.
    /// </summary>
    private float _chargeStart;
    private float _chargeTarget;
    private double _lerpTime;
    private double _targetTime;

    
    private Tween _aplhaTween;

    public override void _Ready()
    {
        _chargeTarget = _charge.Current / _charge.Max; 
        _chargeStart = _chargeTarget;

        _charge.Consumed += Consumed;
    }

    private void Consumed(float charge)
    {
        _chargeTarget = _charge.Current / _charge.Max;
        _chargeStart = _chargeTarget;
        _aplhaTween?.Kill();

        if (Modulate.A == 1f)
            return;

        SetPhysicsProcess(true);
        SetProcess(true);

        _aplhaTween = CreateTween();
        _aplhaTween
            .TweenProperty(this, "modulate:a", 1, _showTime)
            .SetTrans(Tween.TransitionType.Linear)
            .SetEase(Tween.EaseType.InOut);

    }

    public override void _PhysicsProcess(double delta)
    {
        _chargeStart = _chargeTarget;
        _chargeTarget = _charge.Current / _charge.Max;
        _targetTime = delta;
        _lerpTime = 0;
    }

    public override async void _Process(double delta)
    {
        _lerpTime += delta;
        double lerpWeight = _lerpTime/_targetTime;
        Value = Mathf.Lerp(_chargeStart, _chargeTarget, lerpWeight);

        if (Value == 1.0)
        {
            SetProcess(false);
            SetPhysicsProcess(false);
            
            _aplhaTween?.Kill();

            _aplhaTween?.Kill();
            _aplhaTween = CreateTween();
            _aplhaTween
                .TweenProperty(this, "modulate:a", 0, _hideTime)
                .SetTrans(Tween.TransitionType.Linear)
                .SetEase(Tween.EaseType.InOut);

            await ToSignal(_aplhaTween, Tween.SignalName.Finished);
        }
    }
}