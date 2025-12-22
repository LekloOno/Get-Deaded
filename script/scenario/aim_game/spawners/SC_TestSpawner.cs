using System.Collections.Generic;
using Godot;
using System;

namespace Pew;

[GlobalClass]
public partial class SC_TestSpawner : SC_SpawnerScript
{
    // We could make an improved object pooling by having two lists, one for the pool, one for the 
    [Export] private PackedScene _enemy;
    [Export] private uint _count = 4;
    [Export] private float _spawnRadius = 20f;
    [Export] private float _spawnMinDistance = 7f;
    [Export] private float _respawnDelay = 0.52f;
    [Export] private float _roundTime = 30f;
    private Dictionary<E_IEnemy, Timer> _respawnTimers = [];
    private Random _rng = new();
    public SceneTreeTimer RoundTimer;

    public override void _Ready()
    {
        CreateBots();
    }

    private void CreateBots()
    {
        for (int i = 0; i < _count; i++)
        {
            E_Enemy enemy = _enemy.Instantiate<E_Enemy>();
            CreateEnemy(enemy);
        }
    }

    private void SpawnBots()
    {
        foreach (E_IEnemy enemy in Enemies)
            SpawnEnemy(enemy);
    }

    private void Killed(E_IEnemy enemy)
    {
        _respawnTimers[enemy].Start(_respawnDelay);
        RemoveEnemy(enemy);
    }

    protected override void StopSpec()
    {
        RoundTimer.Timeout -= DoStop;
        RoundTimer = null;
        
        foreach (Timer timer in _respawnTimers.Values)
            timer.Stop();

        ClearEnemies();
        foreach (E_IEnemy enemy in Enemies)
            enemy.OnDisable -= Killed;
    }


    private Vector3 RandomPosition()
    {
        Vector3 rot = RotationDegrees;
        Vector3 initRot = rot;
        rot.Y = _rng.NextSingle() * 360;
        RotationDegrees = rot;
        
        float distance = _spawnMinDistance + _rng.NextSingle() * (_spawnRadius - _spawnMinDistance);
        Vector3 rnd = - Basis.Z * distance;
        
        RotationDegrees = initRot;

        return rnd;
    }

    public override void Start(GE_ICombatEntity starter)
    {
        if (RoundTimer != null)
            return;
            
        RoundTimer = GetTree().CreateTimer(_roundTime, false, true);
        RoundTimer.Timeout += DoStop;
        SpawnBots();
        foreach (E_IEnemy enemy in Enemies)
        {
            enemy.OnDisable += Killed;
            enemy.Target = starter;
        }
    }

    protected override void CreateEnemySpec(E_IEnemy enemy)
    {
        if (enemy is not Node node)
            return;

        Timer timer = new() {
            OneShot = true,
            ProcessMode = ProcessModeEnum.Pausable,
            ProcessCallback = Timer.TimerProcessCallback.Physics
        };
        timer.Timeout += () => SpawnEnemy(enemy);
        _respawnTimers.Add(enemy, timer);
        AddChild(timer);
        AddChild(node);
        enemy.Pool();
    }

    protected override void SpawnEnemy(E_IEnemy enemy)
    {
        if (enemy is not E_Enemy node)
            return;

        node.Spawn();
        node.Position = RandomPosition();

        Vector3 target = _player == null
            ? GlobalPosition
            : _player.GlobalPosition;
        
        target.Y = node.GlobalPosition.Y;

        node.LookAt(target);
        
        node.ResetPhysicsInterpolation();
    }

    protected override void RemoveEnemy(E_IEnemy enemy)
    {
        if (!_respawnTimers.TryGetValue(enemy, out Timer timer))
            return;

        enemy.Pool();
    }

    protected override void QueueFreeEnemySpec(E_IEnemy enemy)
    {
        enemy.OnDisable -= Killed;
        _respawnTimers.Remove(enemy, out Timer timer);
        timer?.QueueFree();
    }
}