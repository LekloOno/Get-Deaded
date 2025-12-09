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
    public EventHandler<ShotHitEventArgs> Hit;          // Report hits handled by bots spawned from this script
    public List<E_IEnemy> Enemies {get; private set;} = [];

    /// <summary>
    /// Called when starting this spawn script.
    /// Typically setup the dummies entities.
    /// </summary>
    public abstract void Start();
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

    protected void AddEnemy(E_IEnemy enemy)
    {
        Enemies.Add(enemy);
        enemy.OnDie += _gameManager.HandleKill;
        AddEnemySpec(enemy);
    }

    /// <summary>
    /// Define further behavior specific behavior when adding an enemy, like listenings.
    /// </summary>
    /// <param name="enemy">The added enemy. Not necessarily spawned.</param>
    protected abstract void AddEnemySpec(E_IEnemy enemy);

    protected void RemoveEnemy(E_IEnemy enemy)
    {
        if (!Enemies.Remove(enemy))
            return;
        enemy.OnDie -= _gameManager.HandleKill;
        
        RemoveEnemySpec(enemy);

        if (enemy is not Node node)
            return;

        node.QueueFree();
    }

    /// <summary>
    /// Define further behavior specific behavior when removing an enemy, like unlistenings.
    /// </summary>
    /// <param name="enemy">The added enemy. Not necessarily spawned.</param>
    protected abstract void RemoveEnemySpec(E_IEnemy enemy);

    protected void ClearEnemies()
    {
        foreach (E_IEnemy enemy in Enemies.ToList())
            RemoveEnemy(enemy);
    }
}