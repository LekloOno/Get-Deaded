using System.Collections.Generic;
using Godot;
using System;

[GlobalClass]
public partial class SC_TestSpawner : SC_SpawnerScript
{
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
        //CreateBots();
    }


    private void CreateBots()
    {
        for (int i = 0; i < _count; i++)
        {
            E_Enemy enemy = _enemy.Instantiate<E_Enemy>();
            AddEnemy(enemy);
            AddChild(enemy);
            Spawn(enemy);
        }
    }

    private void Killed(E_IEnemy enemy, GC_Health senderLayer)
    {
        _respawnTimers[enemy].Start();
    }

    protected override void StopSpec()
    {
        RoundTimer.Timeout -= DoStop;
        RoundTimer = null;
        ClearEnemies();
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

    public void Spawn(E_IEnemy enemy)
    {
        if (enemy is not E_Enemy node)
            return;
        
        node.Enable();
        
        node.Position = RandomPosition();

        Vector3 target = _player == null
            ? GlobalPosition
            : _player.GlobalPosition;
        
        target.Y = node.GlobalPosition.Y;

        node.LookAt(target);
        
        node.ResetPhysicsInterpolation();
    }

    public override void Start()
    {
        if (RoundTimer != null)
            return;
            
        RoundTimer = GetTree().CreateTimer(_roundTime);
        RoundTimer.Timeout += DoStop;
        CreateBots();
    }

    protected override void AddEnemySpec(E_IEnemy enemy)
    {
        enemy.OnDie += Killed;

        Timer timer = new() { WaitTime = _respawnDelay, OneShot = true };
        timer.Timeout += () => Spawn(enemy);
        _respawnTimers.Add(enemy, timer);
        AddChild(timer);
    }

    protected override void RemoveEnemySpec(E_IEnemy enemy)
    {
        enemy.OnDie -= Killed;
        _respawnTimers.Remove(enemy, out Timer timer);
        timer?.QueueFree();
    }
}