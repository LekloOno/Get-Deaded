using System.Linq;
using GaudioProcessTree;
using Godot;
using Godot.Collections;

public partial class RunTimerSound : Node
{
    private SC_RunTimer _runTimer = null!;

    [Export] private Array<RunTimerSoundInterval> _intervals = [];
    [Export] private AUD_Sound _beepSound = null!;

    private int _currentIndex = 0;

    public override void _Ready()
    {
        SetProcess(false);

        if (GetParent() is not SC_RunTimer runTimer)
        {
            GD.PushError($"[{nameof(RunTimerSound)}] requires a [{nameof(SC_RunTimer)}] parent.");
            return;
        }

        SortIntervals();

        _runTimer = runTimer;
        _runTimer.Started       += OnStarted;
        _runTimer.Interrupted   += OnInterrupted;
        _runTimer.Timeout       += OnTimeout;
    }

    private void SortIntervals()
    {
        var sorted = _intervals.OrderByDescending(i => i.TriggerTime).ToList();
        _intervals.Clear();
        foreach (var interval in sorted)
            _intervals.Add(interval);
    }

    
    private float _timeLeft = 0f;
    private float _timeSinceLastBeep = 0f;
    public override void _Process(double delta)
    {
        float deltaF = (float) delta;
        _timeLeft -= deltaF;

        if (_timeLeft < 0f)
        {
            Reset();
            return;
        }

        while (_currentIndex < _intervals.Count - 1 &&
               _timeLeft <= _intervals[_currentIndex + 1].TriggerTime)
        {
            _currentIndex++;
        }

        if (_timeLeft > _intervals[0].TriggerTime)
            return;

        _timeSinceLastBeep += deltaF;
        float currentInterval = _intervals[_currentIndex].BeepInterval;

        if (_timeSinceLastBeep >= currentInterval)
        {
            _timeSinceLastBeep = 0f;
            _beepSound.Play();
        }
    }

    private void Reset()
    {
        _currentIndex = 0;
        _timeSinceLastBeep = 0f;
        _timeLeft = 0f;
        SetProcess(false);
    }

    private void OnStarted(float time)
    {
        if (_intervals.Count <= 0)
            return;

        _timeLeft = time;
        _currentIndex = 0;
        _timeSinceLastBeep = _intervals[0].BeepInterval;

        SetProcess(true);
    }

    private void OnInterrupted(GameModeEnd outcome) =>
        Reset();

    private void OnTimeout(GameModeEnd outcome) =>
        Reset();
}