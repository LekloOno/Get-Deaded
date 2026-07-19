using Godot;

public partial class GameModeSignalBridge : Node
{
    private SC_IGameMode _gameMode = null!;

    public override void _Ready()
    {
        if (!SC_IGameModeComponent.RetrieveGameMode(this, out SC_IGameMode? gameMode))
            return;

        _gameMode = gameMode;
        
        _gameMode.Initialized += () => EmitSignal(SignalName.Initialized);
        _gameMode.Started += () => EmitSignal(SignalName.Started);
        _gameMode.Interrupted += (e) => EmitSignal(SignalName.Interrupted, (int) e);
        _gameMode.Reseted += () => EmitSignal(SignalName.Reseted);
    }

    [Signal]
    public delegate void InitializedEventHandler();

	[Signal]
    public delegate void StartedEventHandler();

	[Signal]
    public delegate void InterruptedEventHandler(int outcome);
    
    [Signal]
    public delegate void ResetedEventHandler();
}