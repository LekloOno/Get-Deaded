using System;
using Godot;

[GlobalClass]
public partial class UI_ScoreMarkManager : Control
{
    [Export] private PackedScene _killTemplate;
    [Export] private float _fadeTime;
    
    // Live edit - should later be removed, it's purely to live-tweak the values of _killTemplate
    [Export] private float _trauma = 1f;
    [Export] private float _shakeIntensity = 10f;
    [Export] private ANIM_Vec2TraumaLayer _shakeLayer;
    [Export] public uint MaxChainSize {get; private set;} = 8;
    [Export] private UI_ScoreMarkShaker _scoreMark;
    [Export] private Tween.TransitionType _scoreTrans;
    [Export] private float _scoreTransTime;

    private Tween _scoreTween;

    private uint _accumulatedScore = 0;
    private uint _lerpedAccumulatedScore = 0;
    public uint LerpedAcummulatedScore
    {
        get => _lerpedAccumulatedScore;
        set
        {
            _lerpedAccumulatedScore = value;
            _scoreMark.Text = value + "";
        }
    }


    public Action PushMark;
    public Timer FadeTimer;

    public override void _Ready()
    {
        _scoreMark.Visible = false;

        FadeTimer = new()
        {
            ProcessMode = ProcessModeEnum.Pausable,
            ProcessCallback = Timer.TimerProcessCallback.Physics
        };
        
        AddChild(FadeTimer);

        FadeTimer.Timeout += Reset;
    }

    private void Reset()
    {
        _scoreMark.Fade();
        _accumulatedScore = 0;
    }


    public void OnEarnScore(uint earned, uint score)
    {
        _scoreMark.Visible = true;
        if (_accumulatedScore == 0)
            _scoreMark.Init();
        else
            _scoreMark.Accumulate();

        FadeTimer.Start(_fadeTime);

        _accumulatedScore += earned;

        _scoreTween = CreateTween();
        _scoreTween
            .TweenProperty(this, "LerpedAcummulatedScore", _accumulatedScore, _scoreTransTime)
            .SetTrans(_scoreTrans);
    }

    public void Push() => PushMark?.Invoke();
}