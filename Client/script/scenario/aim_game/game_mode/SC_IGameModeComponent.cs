using System.Diagnostics.CodeAnalysis;
using Godot;

public interface SC_IGameModeComponent : SC_GameModeLifeCycle
{
    SC_IGameMode GameMode {get;}

    public static bool RetrieveGameMode<T>(
        Node node,
        [NotNullWhen(true)] out T? gameMode)
        where T : SC_IGameMode
    {
        Node parent = node.GetParent();
        
        if (parent is T mode)
            gameMode = mode;
        else if (parent is SC_IGameModeComponent modeComponent
            && modeComponent.GameMode is T parentMode)
            gameMode = parentMode;
        else
        {
            gameMode = default;
            GD.PushError($"[{nameof(SC_GenericSpawnerScript)}] should be a child of either [{nameof(SC_IGameMode)}] or [{nameof(SC_IGameModeComponent)}]!");
            return false;
        }

        return true;
    }
}