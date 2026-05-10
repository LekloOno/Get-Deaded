using Godot;

[GlobalClass]
public partial class UI_GlobalScoreTracker : Label
{
    [Export] private Tween.TransitionType _scoreTrans;
    [Export] private float _scoreTransTime;

    private Tween _scoreTween;

    private uint _lerpedScore = 0;
    public uint LerpedScore
    {
        get => _lerpedScore;
        set
        {
            _lerpedScore = value;
            Text = value + "";
        }
    }

    public void Reset()
    {
        LerpedScore = 0;
    }

    public void OnEarnScore(uint earned, uint score)
    {
        _scoreTween = CreateTween();
        _scoreTween
            .TweenProperty(this, "LerpedScore", score, _scoreTransTime)
            .SetTrans(_scoreTrans)
            .SetEase(Tween.EaseType.Out);
    }
}