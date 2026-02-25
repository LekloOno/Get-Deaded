using Godot;

[GlobalClass]
public partial class UI_OmniCharge : TextureProgressBar
{
    [Export] private PM_OmniCharge _charge;
    [Export] private float _showTime;
    [Export] private float _hideTime;
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

    
    private Tween _progressTween;
    private Tween _underTween;

    public override void _Ready()
    {
        _chargeTarget = _charge.Current / _charge.Max; 
        _chargeStart = _chargeTarget;
    }

    public override void _PhysicsProcess(double delta)
    {
        _chargeStart = _chargeTarget;
        _chargeTarget = _charge.Current / _charge.Max;
        _targetTime = delta;
        _lerpTime = 0;
    }

    public override void _Process(double delta)
    {
        _lerpTime += delta;
        double lerpWeight = _lerpTime/_targetTime;
        Value = Mathf.Lerp(_chargeStart, _chargeTarget, lerpWeight);
    }
}