using System.Collections.Generic;
using Godot;
using System;

[GlobalClass]
public partial class SC_SequenceSpawner : SC_SpawnerScript
{
    [Export] private Godot.Collections.Array<E_EnemyBuilder> _enemyBuilders = [];
    [Export] private Godot.Collections.Array<Node3D> _spawnPoints = [];
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
    private readonly List<Timer> _respawnTimers = [];
    private readonly Dictionary<E_IEnemy, int> _enemyToPool = [];
    private readonly Random _rng = new();
    public SceneTreeTimer? RoundTimer;

    private readonly List<Stack<E_IEnemy>> _enemyPools = [];

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

    private void Killed(E_IEnemy enemy, GC_Health _)
    {
        var index = _timerIndex;

        _timerIndex ++;
        _timerIndex %= (int) _count;
        
        _respawnTimers[index].Start(_respawnDelay);
    }

    protected override void StopSpec()
    {
        if (RoundTimer != null)
        {
            RoundTimer.Timeout -= DoStop;
            RoundTimer = null;
        }

        _timerIndex = 0;
        _spawnPoolIndex = 0;
        
        foreach (Timer timer in _respawnTimers)
            timer.Stop();

        foreach (E_IEnemy enemy in Enemies)
        {
            enemy.Disabled -= RemoveEnemy;
            enemy.Died -= Killed;
        }
        ClearEnemies();
    }

    private Vector3 IterateRandomPosition(float maxAttempts)
    {
        var spaceState = GetWorld3D().DirectSpaceState;
        var shape = new SphereShape3D { Radius = 1f };

        Vector3 candidate;
        int attempts = 0;

        do
        {
            candidate = RandomPosition();
            attempts++;

            var query = new PhysicsShapeQueryParameters3D
            {
                Shape = shape,
                Transform = new Transform3D(Basis.Identity, candidate + new Vector3(0f, 0.5f, 0f)),
                CollisionMask = 1
            };

            var overlaps = spaceState.IntersectShape(query);
            if (overlaps.Count == 0)
                return candidate;

        } while (attempts < maxAttempts);

        GD.PushWarning($"[SC_SequenceSpawner] Could not find a free position in {maxAttempts} attemps. Spawning inside collisions.");
        return candidate; 
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

        return ToGlobal(rnd);
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
            enemy.Disabled += RemoveEnemy;
            enemy.Died += Killed;
            enemy.Target = starter;
        }
    }

    protected override void CreateEnemySpec(E_IEnemy enemy)
    {
        if (enemy is not Node node)
            return;

        AddChild(node);
        
        if (_running)
        {
            enemy.Disabled += RemoveEnemy;
            enemy.Died += Killed;
            enemy.Target = Starter;
        }
    }

    private Vector3 GetRandomSpawnPoint()
    {
        int idx = _rng.Next(_spawnPoints.Count);
        return _spawnPoints[idx].GlobalPosition;
    }

    private Vector3 GetSpawnPosition()
    {
        if (_spawnPoints.Count == 0)
            return IterateRandomPosition(MaxSpawnAttempts);
        return GetRandomSpawnPoint();
    }

    private const uint MaxSpawnAttempts = 6;
    protected override void SpawnEnemySpec(E_IEnemy enemy)
    {
        if (enemy is not Node3D node)
            return;

        
        node.GlobalPosition = GetSpawnPosition();

        Vector3 target = Starter == null
            ? GlobalPosition
            : Starter.Body.GlobalTransform.Origin;
        
        target.Y = node.GlobalPosition.Y;

        node.LookAt(target);
        
        node.ResetPhysicsInterpolation();
    }

    protected override void RemoveEnemySpec(E_IEnemy enemy)
    {
        int pool = _enemyToPool[enemy];
        _enemyPools[pool].Push(enemy);
    }

    protected override void QueueFreeEnemySpec(E_IEnemy enemy)
    {
        enemy.Disabled -= RemoveEnemy;
        enemy.Died -= Killed;
        _enemyToPool.Remove(enemy);
    }
}