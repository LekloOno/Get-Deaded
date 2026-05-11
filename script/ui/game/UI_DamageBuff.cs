using Godot;

[GlobalClass]
public partial class UI_DamageBuff : Control
{
    [Export] private PickableSpawner _damageSpawner;
    [Export] private Label _timerLabel;
    [Export] private Label _boostLabel;
    [Export] private Control _vignette;
    [Export] private StyleBoxFlat _vignetteStyle;
    [Export] private float _lowBpm = 50f;
    [Export] private float _highBpm = 150f;
    [Export] private int _maxSize = 50;

    [Export] private Tween.TransitionType _bpmTrans;
    private Tween _bpmTween;

    public float Bpm;

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


    public override void _Ready()
    {
        EndBuff();
        _damageSpawner.PickedUp += PickedUp;
    }

    private void PickedUp(GL_PickableData pickable)
    {
        if (pickable is not GL_DamageBuffData buffData)
            return;

        if (_buffTimer != null)
            _buffTimer.Timeout -= EndBuff;

        _buffTimer = GetTree().CreateTimer(buffData.Duration, false, true);
        _buffTimer.Timeout += EndBuff;

        _timeLeft = buffData.Duration;
        _pulseTime = 0;
        _boostLabel.Text = buffData.Multiplier + "";

        Bpm = _lowBpm;
        _bpmTween?.Kill();
        _bpmTween = CreateTween();
        _bpmTween
            .TweenProperty(this, "Bpm", _highBpm, buffData.Duration)
            .SetTrans(_bpmTrans)
            .SetEase(Tween.EaseType.In);

        Visible = true;
        _vignette.Visible = true;
        SetProcess(true);
    }

    private void EndBuff()
    {
        Visible = false;
        _vignette.Visible = false;
        SetProcess(false);
    }

    public override void _Process(double delta)
    {
        _timeLeft -= delta;

        float bpmRatio = Bpm/_lowBpm;
        _pulseTime += delta * bpmRatio;
        _timerLabel.Text = Mathf.RoundToInt(_timeLeft) + "";
        BorderRadius = Mathf.RoundToInt(HeartPulse((float) _pulseTime, _lowBpm) * _maxSize);
    }

    private float HeartPulse(float time, float bpm = 72f)
    {
        float frequency = bpm / 60f;
        float s = Mathf.Sin(2f * Mathf.Pi * frequency * time);

        return Mathf.Pow(Mathf.Max(0f, s), 10f);
    }
}