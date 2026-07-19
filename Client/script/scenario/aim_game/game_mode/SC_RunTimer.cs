using System;
using Godot;

[GlobalClass]
public partial class SC_RunTimer : Node, SC_IGameModeComponent
{
    public SC_IGameMode GameMode {get; private set;} = null!;
    [Export] private float _seconds = 160f;
    /// <summary>
    /// Defines wether reaching the end of run time is defined as a win or a lose.
    /// 
    /// For example, some mission might require the player to complete something before it times out.
    /// Some other might push the player to survive.
    /// </summary>
    [Export] private bool  _timeOutIsWin = true;
	private SceneTreeTimer? _timer;

    /// <summary>
    /// CountDown has timed out.
    /// </summary>
    public event Action<GameModeEnd>? Timeout;
    /// <summary>
    /// Timer has started.
    /// </summary>
    public event Action<float>? Started;
    /// <summary>
    /// Timer has been interrupted before timeout.
    /// </summary>
    public event Action<GameModeEnd>? Interrupted;

    public override void _Ready()
    {
        if (!SC_IGameModeComponent.RetrieveGameMode(this, out SC_IGameMode? gameMode))
            return;

        GameMode = gameMode;

        GameMode.Started += OnStarted;
        GameMode.Interrupted += OnInterrupted;
    }

    private void OnInterrupted(GameModeEnd outcome) => Interrupt(outcome);
    private void OnStarted() => Start();
    private void OnTimeout()
    {
        ClearTimer();

        GameModeEnd outcome = _timeOutIsWin ? GameModeEnd.Win : GameModeEnd.Lost;
        GameMode.Interrupt(outcome);
        Timeout?.Invoke(outcome);
    }

    public bool Init(GE_IActiveCombatEntity starter) => true;

    public bool Start()
    {
        if (!TryCreateTimer())
            return false;

        Started?.Invoke(_seconds);

        return true;
    }

    public bool Interrupt(GameModeEnd outcome)
    {
        if (!ClearTimer())
            return false;

        Interrupted?.Invoke(outcome);
        return true;
    }

    /// <summary>
    /// Clears and returns wether the timer was running.
    /// </summary>
    /// <returns>Wether it was running.</returns>
    private bool ClearTimer()
    {
        if (_timer is null)
            return false;

        _timer.Timeout -= OnTimeout;
        
        double timeLeft = _timer.TimeLeft;
        _timer = null;

        if (timeLeft <= 0f)
            return false;

        return true;
    }

    /// <summary>
    /// Tries to create the timer and returns whether it is not already running.
    /// </summary>
    /// <returns>Whether it is not already running.</returns>
    private bool TryCreateTimer()
    {
        if (_timer is not null && _timer.TimeLeft > 0f)
            return false;

        _timer = GetTree().CreateTimer(_seconds, false, true);
        _timer.Timeout += OnTimeout;
        return true;
    }
}