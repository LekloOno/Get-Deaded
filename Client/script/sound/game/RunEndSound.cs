using System;
using GaudioProcessTree;
using Godot;

public partial class RunEndSound : Node
{
    private SC_IGameMode _gameMode = null!;

    [Export] private AUD_Sound _winSound    = null!;
    [Export] private AUD_Sound _loseSound   = null!;

    public override void _Ready()
    {
        if (!SC_IGameModeComponent.RetrieveGameMode(this, out SC_IGameMode? gameMode))
            return;

        _gameMode = gameMode;
        _gameMode.Interrupted += OnInterrupted;
    }

    private void OnInterrupted(GameModeEnd end)
    {
        switch (end)
        {
            case GameModeEnd.Lost :
                _loseSound.Play();
                break;
            
            case GameModeEnd.Win :
                _winSound.Play();
                break;
        }
    }
}