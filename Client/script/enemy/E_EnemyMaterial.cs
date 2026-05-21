using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;

[GlobalClass]
public abstract partial class E_EnemyMaterial : Node
{
    protected E_IEnemy? _enemy;
    private NodePath? _enemyPath;
    [Export] public NodePath? EnemyPath
    {
        get => _enemyPath;
        set
        {
            if (_enemyPath == value)
                return;

            _enemyPath = value;
            ResolveEnemy();
            UpdateConfigurationWarnings();
        }        
    }

    public void Oui()
    {
        OnSpawned();
    }

    public override sealed void _Ready()
    {
        if (Engine.IsEditorHint())
            return;

        ResolveEnemy();

        if (_enemy == null)
        {
            GD.PushError("Enemy not set for E_EnemyMaterial.");
            return;
        }

        _enemy.Died += OnDied;
        _enemy.Disabled += OnDisabled;
        _enemy.Damaged += OnDamaged;
        _enemy.Spawned += OnSpawned;

        ReadySpec();
    }

    public abstract Task SmoothDisable(); 

    protected virtual void ReadySpec() {}

    protected abstract void OnSpawned();
    protected abstract void OnDamaged(E_IEnemy enemy, GC_Health senderLayer, DamageEventArgs e);
    protected abstract void OnDisabled(E_IEnemy enemy);
    protected abstract void OnDied(E_IEnemy enemy, GC_Health senderLayer);


    // +-------------------+
    // |  CONFIG WARNINGS  |
    // +-------------------+
    // _____________________

    private void ResolveEnemy()
    {
        _enemy = null;

        if (_enemyPath == null || !IsInsideTree())
            return;

        var node = GetNodeOrNull(_enemyPath);

        if (node is E_IEnemy enemy)
            _enemy = enemy;
    }

    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new List<string>();

        if (_enemyPath == null)
        {
            warnings.Add("No EnemyPath assigned.");
            return [.. warnings];
        }

        var node = GetNodeOrNull(_enemyPath);

        if (node == null)
            warnings.Add("EnemyPath does not resolve to a valid node.");
        else if (node is not E_IEnemy)
            warnings.Add("The node at EnemyPath does not implement E_IEnemy.");

        if (_enemy == null)
            warnings.Add("Resolved enemy is null.");

        return [.. warnings];
    }
}