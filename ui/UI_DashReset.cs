using System;
using System.ComponentModel.Design;
using Godot;

public partial class UI_DashReset : TextureProgressBar
{
    [Export] private PM_Dash _dash;
    [Export] private float _timeToShow;
    [Export] private float _timeToHide;
    [Export] private float _timeToBlinkIn;
    [Export] private float _timeToBlinkOut;

    [Export(PropertyHint.Range, "0.0, 1.0")]
    private float _baseOpacity;
    [Export(PropertyHint.Range, "0.0, 1.0")]
    private float _underBaseOpacity;

    private bool _active = false;

    private float _remainingTime;
    private float _timeSpent;
    private Tween _progressTween;
    private Tween _underTween;

    public override void _Ready()
    {
        _dash.OnTryReset += StartReset;
        _dash.OnUnavailable += Unavailable;

        HideAlpha();

        SetPhysicsProcess(false);
    }
    
    public override void _PhysicsProcess(double delta)
    {
        float progress = _timeSpent/_remainingTime;

        if (progress > 1f)
            EndAnimation();
        else
        {
            Value = progress;
            _timeSpent += (float) delta;
        }
    }

    public void StartReset(object sender, float remainingTime)
    {
        if(remainingTime == 0f)
        {
            EndAnimation();
            return;
        }

        if(_active)
            return;

        _timeSpent = 0f;
        _remainingTime = remainingTime;

        _active = true;
        _progressTween = CreateTween();
        _progressTween.TweenProperty(this, "tint_progress:a", _baseOpacity, _timeToShow).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);

        _underTween = CreateTween();
        _underTween.TweenProperty(this, "tint_under:a", _underBaseOpacity, _timeToShow).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);
        SetPhysicsProcess(true);
    }

    public void Unavailable(object sender, EventArgs e)
    {
        _progressTween?.Kill();

        _progressTween = CreateTween();
        _progressTween.TweenProperty(this, "tint_progress:a", 1f, _timeToBlinkIn).SetTrans(Tween.TransitionType.Linear);
        _progressTween.TweenProperty(this, "tint_progress:a", _baseOpacity, _timeToBlinkOut).SetTrans(Tween.TransitionType.Linear);
    }

    private void EndAnimation()
    {
        _active = false;
        SetPhysicsProcess(false);

        Value = 1f;

        ResetAlpha();

        
        _progressTween?.Kill();
       
        _progressTween = CreateTween();
        _progressTween.TweenProperty(this, "tint_progress:a", 0f, _timeToHide).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);

        _underTween?.Kill();

        _underTween = CreateTween();
        _underTween.TweenProperty(this, "tint_under:a", 0f, _timeToHide).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);

    }

    private void HideAlpha()
    {
        SetProgressAlpha(0f);
        SetUnderAlpha(0f);
    }

    private void ResetAlpha()
    {
        SetProgressAlpha(_baseOpacity);
        SetUnderAlpha(_underBaseOpacity);
    }

    private void SetProgressAlpha(float alpha)
    {
        Color progress = TintProgress;
        progress.A = alpha;
        TintProgress = progress;
    }

    private void SetUnderAlpha(float alpha)
    {
        Color under = TintUnder;
        under.A = alpha;
        TintUnder = under;
    }

}