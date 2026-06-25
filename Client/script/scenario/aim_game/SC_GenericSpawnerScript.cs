using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public abstract partial class SC_GenericSpawnerScript : Node3D
{
    [Export] protected SC_GameManager _gameManager = null!;
    [Signal] public delegate void StopEventHandler();   // Called at the end of this scene script
    [Signal] public delegate void HandleNextEventHandler(SC_GenericSpawnerScript prev);
    public GE_IActiveCombatEntity Starter {get; private set;} = null!;
    
    protected readonly List<E_IEnemy> SpawnedEnemies = [];

    protected bool _running;

    public void Start(GE_IActiveCombatEntity starter)
    {
        if (_running)
            return;

        _running = true;
        _gameManager.Interrupt += Interrupt;
        Starter = starter;
        StartSpec(starter);
    }

    public void Interrupt()
    {
        if (!_running)
            return;

        _gameManager.Interrupt -= Interrupt;
        Clear();
        StopSpec();
        _running = false;
    }

    public void DoStop()
    {   
        if (!_running)
            return;

        Interrupt();
        EmitSignal(SignalName.Stop);
        EmitSignal(SignalName.HandleNext, this);
    }

    protected abstract void StartSpec(GE_IActiveCombatEntity starter);
    protected abstract void StopSpec();

    protected E_IEnemy GetEnemy(E_EnemyBuilder builder)
    {
        E_IEnemy enemy = EnemyPoolServer.Request(builder);

        enemy.Disabled  += OnEnemyDisabled;
        enemy.Died      += OnEnemyDied;
        enemy.Died      += _gameManager.HandleKill;
        enemy.Pooled    += OnEnemyPooled;

        SpawnedEnemies.Add(enemy);
        enemy.Target = Starter;
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
        enemy.Died      -= _gameManager.HandleKill;
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
}