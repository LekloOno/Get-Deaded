using System.Collections.Generic;
using Godot;
using System;

[GlobalClass]
public partial class SC_SequenceSpawner : SC_SpawnerScript
{
    [Export] private Godot.Collections.Array<E_EnemyBuilder> _enemyBuilders;
    [Export] private uint _count = 4;
    [Export] private float _spawnRadius = 20f;
    [Export] private float _spawnMinDistance = 7f;
    [Export] private float _respawnDelay = 0.52f;
    [Export] private float _roundTime = 30f;
    /// <summary>
    /// Multiple enemies might need to respawn at once.
    /// At most, all enemies are dead, waiting for respawn, so we need _count different timers.
    /// The timer are not tied to a specific enemy. They are only tied when they are active, but can be tied to any enemy.
    /// </summary>
    private List<Timer> _respawnTimers = [];
    private Dictionary<E_IEnemy, int> _enemyToPool = [];
    private Random _rng = new();
    public SceneTreeTimer RoundTimer;

    private List<Stack<E_IEnemy>> _enemyPools = [];

    private int _spawnPoolIndex;
    private int _timerIndex;

    public override void _Ready()
    {
        CreateBots();
    }

    /// <summary>
    /// Preamble function. Not related to actual round/game.
    /// It creates and preallocates the bots in pools, so they don't have to be on-the-go.
    /// </summary>
    private void CreateBots()
    {
        for (int i = 0; i < _count; i ++)
        {
            Timer timer = new() {
                OneShot = true,
                ProcessMode = ProcessModeEnum.Pausable,
                ProcessCallback = Timer.TimerProcessCallback.Physics
            };
            timer.Timeout += SpawnNextEnemy;

            _respawnTimers.Add(timer);
            AddChild(timer);
        }

        for (int poolIndex = 0; poolIndex < _enemyBuilders.Count; poolIndex ++)
        {
            E_EnemyBuilder builder = _enemyBuilders[poolIndex];
            Stack<E_IEnemy> pool = [];
            _enemyPools.Add([]);
            for (int i = 0; i < _count; i++)
            {
                E_Enemy enemy = builder.Build();
                _enemyToPool.Add(enemy, poolIndex);
                _enemyPools[poolIndex].Push(enemy);
                CreateEnemy(enemy);
                
            }
        }

        
    }

    private void SpawnBots()
    {
        for (int i = 0; i < _count; i ++)
            SpawnNextEnemy();
    }

    private void SpawnNextEnemy()
    {
        int index = _spawnPoolIndex;
        _spawnPoolIndex ++;
        _spawnPoolIndex %= _enemyBuilders.Count;

        if (!_enemyPools[index].TryPop(out E_IEnemy? enemy))
        {
            E_IEnemy newEnemy = _enemyBuilders[index].Build();
            _enemyToPool.Add(newEnemy, index);
            CreateEnemy(newEnemy);
            enemy = newEnemy;
        }

        
        SpawnEnemy(enemy);
    }

    private void OnDisabled(E_IEnemy enemy)
    {
        int pool = _enemyToPool[enemy];
        _enemyPools[pool].Push(enemy);
    }

    private void Killed(E_IEnemy enemy, GC_Health _)
    {
        var index = _timerIndex;

        _timerIndex ++;
        _timerIndex %= (int) _count;
        
        _respawnTimers[index].Start(_respawnDelay);
    }

    protected override void StopSpec()
    {
        RoundTimer.Timeout -= DoStop;
        RoundTimer = null;

        _timerIndex = 0;
        _spawnPoolIndex = 0;
        
        foreach (Timer timer in _respawnTimers)
            timer.Stop();

        ClearEnemies();
        foreach (E_IEnemy enemy in Enemies)
        {
            enemy.OnDisable -= OnDisabled;
            enemy.OnDie -= Killed;
        }
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

    protected override void StartSpec(GE_ICombatEntity starter)
    {
        if (RoundTimer != null)
            return;
            
        RoundTimer = GetTree().CreateTimer(_roundTime, false, true);
        RoundTimer.Timeout += DoStop;
        SpawnBots();
        foreach (E_IEnemy enemy in Enemies)
        {
            enemy.OnDisable += OnDisabled;
            enemy.OnDie += Killed;
            enemy.Target = starter;
        }
    }

    protected override void CreateEnemySpec(E_IEnemy enemy)
    {
        if (enemy is not Node node)
            return;

        AddChild(node);
        enemy.Pool();
    }

    protected override void SpawnEnemy(E_IEnemy enemy)
    {
        if (enemy is not Node3D node)
            return;

        enemy.Spawn();
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
        int pool = _enemyToPool[enemy];
        _enemyPools[pool].Push(enemy);
        enemy.Pool();
    }

    protected override void QueueFreeEnemySpec(E_IEnemy enemy)
    {
        enemy.OnDisable -= OnDisabled;
        enemy.OnDie -= Killed;
        _enemyToPool.Remove(enemy);
    }
}