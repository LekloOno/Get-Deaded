using Godot;

[GlobalClass]
public partial class UI_ArenaCountDown : Node
{
    [Export] private Label _num;
    [Export] private Tween.TransitionType _scaleTrans;
    [Export] private float _maxScale;
    [Export] private float _scaleTime;
    [Export] private Tween.TransitionType _opacityTrans;
    [Export] private float _opacityTime;

    private Tween _scaleTween;
    
    private Tween _opacityTween;
    private int _secondsLeft = 0;

    public Timer CountDownTimer;

    public override void _Ready()
    {
        _num.Visible = false;
        
        CountDownTimer = new()
        {
            ProcessMode = ProcessModeEnum.Pausable,
            ProcessCallback = Timer.TimerProcessCallback.Physics,
        };
        
        AddChild(CountDownTimer);

        CountDownTimer.Timeout += AnimNextSecond;
    }

    public void OnGameInit(SC_GameManager manager) =>
        StartCountDown(manager.CountDown);

    private void StartCountDown(float countDown)
    {
        _num.Visible = true;
        _secondsLeft = Mathf.FloorToInt(countDown);
        float partSeconds = countDown % 1f;
        if (partSeconds != 0)
            CountDownTimer.Start(partSeconds);
        else
            AnimNextSecond();
    }

    private void AnimNextSecond()
    {
        _num.Text = _secondsLeft + "";
        _secondsLeft --;

        _num.Scale = new Vector2(_maxScale, _maxScale);

        _scaleTween = CreateTween();
        _scaleTween
            .TweenProperty(_num, "scale", Vector2.One, _scaleTime)
            .SetTrans(_scaleTrans);
        
        Color mod = _num.Modulate;
        mod.A = 0f;
        _num.Modulate = mod;

        _opacityTween = CreateTween();
        _opacityTween
            .TweenProperty(_num, "modulate:a", 1f, _opacityTime)
            .SetTrans(_opacityTrans);

        if (_secondsLeft >= 0f)
            CountDownTimer.Start(1f);
        else
            _num.Visible = false;
    }
}