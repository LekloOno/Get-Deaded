using System.Collections.Generic;
using Godot;
using System;

[GlobalClass]
public partial class SC_AimArenaSpawner : Node3D
{
    [Export] private PackedScene _enemy;
    [Export] private uint _count = 4;
    [Export] private float _spawnRadius = 20f;
    [Export] private float _spawnMinDistance = 7f;
    [Export] private float _respawnDelay = 0f;
    [Export] private float _roundTime = 30f;
    [Export] private float _countDown = 2f;
    private List<E_Enemy> _enemies = [];
    private List<Timer> _respawnTimers = [];
    private Random _rng = new();
    private PM_Controller _player;
    public SceneTreeTimer RoundTimer;
    public SceneTreeTimer CountDownTimer;
    private uint _kills = 0;

    public override void _Ready()
    {
        CreateBots();
    }

    public void InitGame(PM_Controller player)
    {
        //ClearBots();
        _kills = 0;
        _player = player;
        _player?.WeaponsHandler.DisableFire();

        CountDownTimer = GetTree().CreateTimer(_countDown, false, true);
        CountDownTimer.Timeout += StartGame;

        //CreateBots();
    }

    private void CreateBots()
    {
        for (int i = 0; i < _count; i++)
        {
            int id = i;

            Timer timer = new() { WaitTime = _respawnDelay, OneShot = true };

            AddChild(timer);

            timer.Timeout += () => Respawn(id);
            _respawnTimers.Add(timer);

            E_Enemy enemy = _enemy.Instantiate<E_Enemy>();

            AddChild(enemy);

            Spawn(enemy);
            enemy.OnDie += (_, _) => Killed(id);

            _enemies.Add(enemy);
        }
    }

    private void ClearBots()
    {
        foreach (E_Enemy enemy in _enemies)
            enemy.QueueFree();

        foreach (Timer timer in _respawnTimers)
            timer.QueueFree();

        _enemies.Clear();
        _respawnTimers.Clear();
    }

    private void Killed(int id)
    {
        _kills ++;
        _respawnTimers[id].Start();
    }

    private void StartGame()
    {
        CountDownTimer.Timeout -= StartGame;

        _player?.WeaponsHandler.EnableFire();
        
        RoundTimer = GetTree().CreateTimer(_roundTime, false, true);
        RoundTimer.Timeout += StopGame;
    }

    private void StopGame()
    {
        RoundTimer.Timeout -= StopGame;
        //GD.Print("Got " + _kills + " kills !");
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

    public void Spawn(E_Enemy enemy)
    {
        enemy.Spawn();
        
        enemy.Position = RandomPosition();

        Vector3 target = _player == null
            ? GlobalPosition
            : _player.GlobalPosition;
        
        target.Y = enemy.GlobalPosition.Y;

        enemy.LookAt(target);
        
        enemy.ResetPhysicsInterpolation();
    }

    public void Respawn(int id)
    {
        E_Enemy enemy = _enemies[id];
        Spawn(enemy);
    }
}