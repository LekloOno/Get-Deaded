using System.Collections.Generic;
using Godot;

public partial class EnemyPoolServer : Node
{
    public static EnemyPoolServer Instance {get; private set;} = null!;

    public override void _Ready()
    {
        Instance = this;
    }

    private readonly Dictionary<E_EnemyBuilder, Stack<E_IEnemy>> _pools = [];

    public static void Alloc(E_EnemyBuilder builder, uint count)
    {
        if (!Instance._pools.TryGetValue(builder, out Stack<E_IEnemy>? pool))
        {
            pool = [];
            Instance._pools.Add(builder, pool);
        }
            
        for (int i = pool.Count; i < count; i++)
            Create(builder, pool).Pool();
    }

    public static E_IEnemy Request(E_EnemyBuilder builder)
    {
        E_IEnemy enemy;
        if (!Instance._pools.TryGetValue(builder, out Stack<E_IEnemy>? pool))
        {
            pool = [];
            Instance._pools.Add(builder, pool);
            enemy = Create(builder, pool);
        }
        
        else if (!pool.TryPop(out enemy!))
            enemy = Create(builder, pool);
        
        enemy.Spawn();

        
        return enemy;
    }

    private static E_IEnemy Create(E_EnemyBuilder builder, Stack<E_IEnemy> pool)
    {
        E_Enemy enemy = builder.Build();
        if (enemy is Node node)
            Instance.AddChild(node);

        enemy.Pooled += pool.Push;
        //_alocated ++;
        //DebugPool();
        return enemy;
    }

    private static int _alocated = 0;
    private static void DebugPool()
    {
        GD.Print("===============");
        int tot = 0;
        foreach (Stack<E_IEnemy> pool in Instance._pools.Values)
        {
            tot += pool.Count;
            GD.Print($"pool count : {pool.Count}");
        }

        GD.Print($"Pooled Total: {tot}");
        GD.Print($"Alocated Total: {_alocated}");
        GD.Print("===============");
    }
}