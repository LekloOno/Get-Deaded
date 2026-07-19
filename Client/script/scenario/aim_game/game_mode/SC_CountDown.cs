using System;
using Godot;

[GlobalClass]
public partial class SC_CountDown : Node, SC_IGameModeComponent
{
    public SC_IGameMode GameMode {get; private set;} = null!;
	[Export] public float _countDown = 2f;
	private SceneTreeTimer? _timer;

    /// <summary>
    /// CountDown has timed out.
    /// </summary>
    public event Action? Timeout;
    /// <summary>
    /// CountDown has started.
    /// </summary>
    public event Action<float>? Started;
    /// <summary>
    /// CountDown got interrupted before timeout.
    /// </summary>
    public event Action? Interrupted;

    private void OnTimeout()
    {
        GameMode.Start();
        Timeout?.Invoke();
    }

    public override void _Ready()
    {
        if (!SC_IGameModeComponent.RetrieveGameMode(this, out SC_IGameMode? gameMode))
            return;

        GameMode = gameMode;

        GameMode.Initialized += OnInitialized;
        GameMode.Interrupted += OnInterrupted;
    }

    private void OnInterrupted(GameModeEnd outcome) => Interrupt(outcome);

    private void OnInitialized() => Start();

    public bool Start()
    {
        if (_timer is not null && _timer.TimeLeft > 0f)
            return false;

        _timer = GetTree().CreateTimer(_countDown, false, true);
        _timer.Timeout += OnTimeout;

        Started?.Invoke(_countDown);

        return true;
    }

    public bool Interrupt()
    {
        if (_timer is null || _timer.TimeLeft <= 0f)
            return false;

        _timer.Timeout -= OnTimeout;
        _timer = null;

        Interrupted?.Invoke();

        return true;
    }

    public bool Init(GE_IActiveCombatEntity starter) =>
        Start();

    public bool Interrupt(GameModeEnd outcome) =>
        Interrupt();
}