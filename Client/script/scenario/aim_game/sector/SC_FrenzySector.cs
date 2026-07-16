using System;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class SC_FrenzySector : SC_LeafSector
{
    [Export] private uint _simultaneous;
    [Export] private uint _count;
    [Export] private Array<DATA_FrenzyEntry> _enemyBuilders = null!;
    
    private int _builderIndex;
    private int _builderCount;
    private int _killedCount;
    private int _spawnedCount;

    private readonly Random _rng = new();

    protected override void LeafReadySpec()
    {
        _availableNodes.AddRange([.._spawnPoints]);

        foreach (DATA_FrenzyEntry data in _enemyBuilders)
            EnemyPoolServer.Alloc(data.Builder, Math.Min(_simultaneous, data.Count));
    }

    protected override void OnEnemyCleared(E_IEnemy enemy) {}
    protected override void OnEnemyDiedSpec(E_IEnemy enemy, GC_Health? senderLayer)
    {
        _killedCount ++;
        if (_killedCount >= _count)
            DoStop();
        else
            SpawnNext();
    }

    private E_EnemyBuilder GetBuilder()
    {
        DATA_FrenzyEntry data = _enemyBuilders[_builderIndex];
        _builderCount ++;
        
        if (_builderCount >= data.Count)
        {
            _builderIndex ++;
            _builderIndex %= _enemyBuilders.Count;

            _builderCount = 0;
        }

        return data.Builder;
    }

    protected override void OnEnemyDisabledSpec(E_IEnemy enemy) {}
    protected override void OnEnemyPooledSpec(E_IEnemy enemy) {}
    protected override void StartSpec(GE_IActiveCombatEntity starter)
    {
        _builderIndex = 0;
        _builderCount = 0;
        _spawnedCount = 0;
        _killedCount  = 0;

        for (int i = 0; i < _simultaneous; i ++)
            SpawnNext();
    }

    protected override void StopSpec() {}

    private void SpawnNext()
    {
        if (_spawnedCount >= _count)
            return;

        E_IEnemy enemy = GetEnemy(GetBuilder());
        enemy.Body.Teleport(GetRandomSpawnPoint());

        _spawnedCount ++;
    }

    private Vector3 GetRandomSpawnPoint()
    {
        FlushReservations();

        int idx = _rng.Next(_availableNodes.Count);
        Node3D node = _availableNodes[idx];
        _reservedNodes.Add(node);
        _availableNodes.RemoveAt(idx);
        return node.GlobalPosition;
    }

    private readonly List<Node3D>  _availableNodes = [];
    private readonly List<Node3D>  _reservedNodes = [];
    private int _reservedFlushFrame = -1;

    private void FlushReservations()
    {
        int frame = (int)Engine.GetPhysicsFrames();
        if (frame != _reservedFlushFrame)
        {
            _availableNodes.AddRange([.._reservedNodes]);
            _reservedFlushFrame = frame;
        }
    }

    protected override void InitSpec() {}
}