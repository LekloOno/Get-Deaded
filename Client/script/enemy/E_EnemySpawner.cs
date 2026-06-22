using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class E_EnemySpawner : Node3D
{
    [Export] private PackedScene _enemy = null!;

    [Export] private float _respawnDelay;
    private SceneTreeTimer? _respawnTimer;
    private SceneTreeTimer? _hideTimer;

    private readonly Stack<E_IEnemy> _pool = [];

    const uint Prealloc = 3;

    public override void _Ready()
    {
        InitEnemies();
        Respawn();
    }

    private void InitEnemies()
    {
        for (int i = 0; i < Prealloc; i ++)
        {
            E_IEnemy enemy = CreateEnemy();

            if (enemy is not Node node)
                return;

            AddChild(node);

            enemy.Spawn();
            enemy.Pool();
            _pool.Push(enemy);
        }
    }

    private E_IEnemy CreateEnemy()
    {
        E_IEnemy enemy = _enemy.Instantiate<E_IEnemy>();

        enemy.Died += OnDied;
        enemy.Disabled += OnDisabled;

        return enemy;
    }


    public void Respawn()
    {
        E_IEnemy enemy;

        if (!_pool.TryPop(out enemy!))
            enemy = CreateEnemy();

        enemy.Body.Teleport(GlobalPosition);
        enemy.Spawn();
    }

    public void OnDied(E_IEnemy enemy, GC_Health senderLayer)
    {
        _respawnTimer = GetTree().CreateTimer(_respawnDelay, false, true);
        _respawnTimer.Timeout += Respawn;
    }
    public void OnDisabled(E_IEnemy enemy)
    {
        _pool.Push(enemy);
    }
}