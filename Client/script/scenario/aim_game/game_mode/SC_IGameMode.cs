using System;

public interface SC_IGameMode : SC_GameModeLifeCycle
{
    GE_IActiveCombatEntity? Player {get;}
    void HandleKill(E_IEnemy enemy, GC_Health senderLayer);

    event Action? Initialized;
    event Action? Started;
    event Action<GameModeEnd>? Interrupted;
    event Action? Reseted;

    event Action<E_IEnemy, GC_Health>? EnemyKilled;
}

public enum GameModeEnd
{
    Lost,
    Win,
    Surrender,
}