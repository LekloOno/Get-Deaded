using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public abstract partial class SC_GenericSpawnerScript : Node3D, SC_IGameModeComponent
{
    public SC_IGameMode GameMode {get; private set;} = null!;
    public event Action? Initialized;
    public event Action? Stopped;
    public event Action<GameModeEnd>? Interrupted;
    public event Action<SC_GenericSpawnerScript>? HandlingPassed;
    
    protected readonly List<E_IEnemy> SpawnedEnemies = [];

    protected bool _running;

    public sealed override void _EnterTree()
    {
        if (!SC_IGameModeComponent.RetrieveGameMode(this, out SC_IGameMode? gameMode))
            return;

        GameMode = gameMode;
    }

    public override sealed void _Ready()
    {
        ReadySpec();
    }

    public bool Init(GE_IActiveCombatEntity starter)
    {
        InitSpec(starter);
        Initialized?.Invoke();
        return true;
    }

    protected abstract void InitSpec(GE_IActiveCombatEntity starter);

    public bool Start()
    {
        if (_running)
            return false;

        _running = true;
        return StartSpec();
    }

    public bool Interrupt(GameModeEnd outcome)
    {
        if (!_running)
            return false;

        if (!InterruptSpec(outcome))
            return false;

        Clear();
        _running = false;
        Interrupted?.Invoke(outcome);
        return true;
    }

    /// <summary>
    /// Normal stop, completed the script.
    /// </summary>
    public void DoStop()
    {   
        if (!_running)
            return;

        Interrupt(GameModeEnd.Win);
        Stopped?.Invoke();
        HandlingPassed?.Invoke(this);
    }

    protected abstract bool StartSpec();
    protected abstract bool InterruptSpec(GameModeEnd outcome);

    protected E_IEnemy GetEnemy(E_EnemyBuilder builder)
    {
        E_IEnemy enemy = EnemyPoolServer.Request(builder);

        enemy.Disabled  += OnEnemyDisabled;
        enemy.Died      += OnEnemyDied;
        enemy.Died      += GameMode.HandleKill;
        enemy.Pooled    += OnEnemyPooled;

        SpawnedEnemies.Add(enemy);
        enemy.Target = GameMode.Player;
        return enemy;
    }

    protected void Clear()
    {
        foreach (E_IEnemy enemy in SpawnedEnemies.ToList())
            ClearEnemy(enemy);
    }

    private void ClearEnemy(E_IEnemy enemy)
    {
        if (enemy.Alive)
            enemy.Pool();
            
        OnEnemyCleared(enemy);
    }

    private void OnEnemyPooled(E_IEnemy enemy)
    {
        enemy.Disabled  -= OnEnemyDisabled;
        enemy.Died      -= OnEnemyDied;
        enemy.Died      -= GameMode.HandleKill;
        enemy.Pooled    -= OnEnemyPooled;

        SpawnedEnemies.Remove(enemy);
        OnEnemyPooledSpec(enemy);
    }

    private void OnEnemyDisabled(E_IEnemy enemy)
    {
        //GD.Print("ALO DISABLED FFS");
        enemy.Pool();
        OnEnemyDisabledSpec(enemy);
    }

    private void OnEnemyDied(E_IEnemy enemy, GC_Health? senderLayer)
    {
        OnEnemyDiedSpec(enemy, senderLayer);
    }

    protected abstract void OnEnemyDisabledSpec(E_IEnemy enemy);
    protected abstract void OnEnemyDiedSpec(E_IEnemy enemy, GC_Health? senderLayer);
    protected abstract void OnEnemyPooledSpec(E_IEnemy enemy);
    protected abstract void OnEnemyCleared(E_IEnemy enemy);

    protected abstract void ReadySpec();
}