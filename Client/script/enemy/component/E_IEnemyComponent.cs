using System.Collections.Generic;
using Godot;

public interface E_IEnemyComponent
{
    NodePath EnemyPath   {get; set;}
    E_IEnemy? Enemy      {get; set;}

    void OnDied(E_IEnemy enemy, GC_Health senderLayer);
    void OnPooled(E_IEnemy enemy);
    void OnSpawned();
    void OnDisabled(E_IEnemy enemy);
    void OnEnemyChanged(E_IEnemy? prev, E_IEnemy? next);

    public bool ResolveEnemy(Node node)
    {
        Enemy = null;
        if (EnemyPath == null || !node.IsInsideTree())
            return false;

        var enemyNode = node.GetNodeOrNull(EnemyPath);

        if (enemyNode is not E_IEnemy validEnemy)
            return false;

        if (Enemy == validEnemy)
            return true;

        if (Enemy != null)
        {
            Enemy.Died      -= OnDied;
            Enemy.Disabled  -= OnDisabled;
            Enemy.Spawned   -= OnSpawned;
            Enemy.Pooled    -= OnPooled;
            
        }

        E_IEnemy? prev = Enemy;
        Enemy = validEnemy;
        Enemy.Died      += OnDied;
        Enemy.Disabled  += OnDisabled;
        Enemy.Spawned   += OnSpawned;
        Enemy.Pooled    += OnPooled;

        OnEnemyChanged(prev, Enemy);
        return true;
    }

    public string[] GetConfigurationWarnings(Node baseNode)
    {
        var warnings = new List<string>();

        if (EnemyPath == null)
        {
            warnings.Add("No EnemyPath assigned.");
            return [.. warnings];
        }

        var node = baseNode.GetNodeOrNull(EnemyPath);

        if (node == null)
            warnings.Add("EnemyPath does not resolve to a valid node.");
        else if (node is not E_IEnemy)
            warnings.Add("The node at EnemyPath does not implement E_IEnemy.");

        if (Enemy == null)
            warnings.Add("Resolved enemy is null.");

        return [.. warnings];
    }
}