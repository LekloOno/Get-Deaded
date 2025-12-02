using System.Collections.Generic;
using Godot;
using System;

[GlobalClass]
public partial class SC_AimArenaSpawner : Node3D
{
    [Export] private PackedScene _enemy;
    [Export] private uint _count = 4;
    [Export] private float _spawnRadius = 20f;
    [Export] private float _respawnDelay = 0f;
    private E_Enemy _caca;
    private Timer _cacaTimer;
    private List<E_Enemy> _enemies = [];
    private List<Timer> _respawnTimers = [];
    private Random _rng = new();

    public override void _Ready()
    {
        
        for (int i = 0; i < _count; i++)
        {
            int id = i;

            Timer timer = new(){WaitTime = _respawnDelay, OneShot = true};

            AddChild(timer);

            timer.Timeout += () => Respawn(id);
            _respawnTimers.Add(timer);

            E_Enemy enemy = _enemy.Instantiate<E_Enemy>();

            AddChild(enemy);
            
            enemy.Position = RandomPosition();
            enemy.OnDie += (senderLayer) => _respawnTimers[id].Start();

            _enemies.Add(enemy);
        }
    }

    private Vector3 RandomPosition()
    {
        Vector3 rot = RotationDegrees;
        Vector3 initRot = rot;
        rot.Y = _rng.NextSingle() * 360;
        RotationDegrees = rot;
        
        Vector3 rnd = - Basis.Z * _spawnRadius;
        
        RotationDegrees = initRot;

        return rnd;
    }

    public void Respawn(int id)
    {
        E_Enemy enemy = _enemies[id];
        enemy.Position = RandomPosition();
        enemy.ResetPhysicsInterpolation();
        enemy.Enable();
    }
}