using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public abstract partial class SC_SpawnerScript : Node3D
{
    [Export] protected SC_GameManager _gameManager;
    protected PM_Controller _player;
    [Signal] public delegate void StopEventHandler();   // Called at the end of this scene script
    public EventHandler<HitEventArgs> Hit;          // Report hits handled by bots spawned from this script
    public List<E_IEnemy> Enemies {get; private set;} = [];

    public override void _EnterTree()
    {
        _gameManager.Interrupt -= StopSpec;
        _gameManager.Interrupt += StopSpec;
    }

    /// <summary>
    /// Called when starting this spawn script.
    /// Typically setup the dummies entities.
    /// </summary>
    public abstract void Start(GE_ICombatEntity starter);
    public void DoStop()
    {   
        EmitSignal(SignalName.Stop);
        StopSpec();
    }
    
    protected abstract void StopSpec();
    
    public void Init(PM_Controller player)
    {
        _player = player;
    }

    protected void SetTarget(E_IEnemy enemy, GE_ICombatEntity target)
    {
        enemy.Target = target;
    }

    /// <summary>
    /// Define what should be done to enable an enemy that has been previously created through CreateEnemy. <br/>
    /// Typically, adding it to the tree.
    /// </summary>
    /// <param name="enemy"></param>
    protected abstract void SpawnEnemy(E_IEnemy enemy);
    /// <summary>
    /// Define further specific behavior when creating a new enemy, like subscribing to some events.
    /// </summary>
    /// <param name="enemy"></param>
    protected abstract void CreateEnemySpec(E_IEnemy enemy);

    /// <summary>
    /// Define what should be done to disable an enemy that has previously been created through CreateEnemy.
    /// For example - removing it from the tree, unsubscribing some events, etc. <br/>
    /// <br/>
    /// This is not specific to a queue free of the enemy but it is also called when freeing. <br/>
    /// For free specifics, see QueueFreeEnemy and QueueFreeEnemySpec.
    /// </summary>
    /// <param name="enemy"></param>
    protected abstract void RemoveEnemy(E_IEnemy enemy);
    /// <summary>
    /// Define further specific behavior when destroying an existing enemy, like freeing external associated nodes.
    /// </summary>
    /// <param name="enemy"></param>
    protected abstract void QueueFreeEnemySpec(E_IEnemy enemy);

    protected void CreateEnemy(E_IEnemy enemy)
    {
        Enemies.Add(enemy);
        enemy.OnDie += _gameManager.HandleKill;
        
        CreateEnemySpec(enemy);
    }

    protected void QueueFreeEnemy(E_IEnemy enemy)
    {
        RemoveEnemy(enemy);

        if (!Enemies.Remove(enemy))
            return;
        enemy.OnDie -= _gameManager.HandleKill;
        
        QueueFreeEnemySpec(enemy);

        if (enemy is not Node node)
            return;

        node.QueueFree();
    }

    protected void ClearEnemies()
    {
        foreach (E_IEnemy enemy in Enemies.ToList())
            RemoveEnemy(enemy);
    }

    protected void ClearAndFreeEnemies()
    {
        foreach (E_IEnemy enemy in Enemies.ToList())
            QueueFreeEnemy(enemy);
    }
}