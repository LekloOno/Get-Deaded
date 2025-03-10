using System;
using System.ComponentModel.Design;
using Godot;

public partial class UI_DashReset : TextureProgressBar
{
    [Export] private PM_Dash _dash;
    [Export] private float _timeToShow;
    [Export] private float _timeToHide;

    private bool _active = false;

    private float _remainingTime;
    private float _timeSpent;
    private Tween _showTween;

    public override void _Ready()
    {
        _dash.OnTryReset += StartReset;

        Color color = Modulate;
        color.A = 0;
        Modulate = color;

        SetPhysicsProcess(false);
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
        _showTween = CreateTween();
        _showTween.TweenProperty(this, "modulate:a", 1f, _timeToShow).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);
        SetPhysicsProcess(true);
    }

    private void EndAnimation()
    {
        _active = false;
        SetPhysicsProcess(false);
        
        Value = 1f;

        Color color = Modulate;
        color.A = 1f;
        Modulate = color;

        
        _showTween?.Kill();
       
        _showTween = CreateTween();
        _showTween.TweenProperty(this, "modulate:a", 0f, _timeToHide).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);
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
}