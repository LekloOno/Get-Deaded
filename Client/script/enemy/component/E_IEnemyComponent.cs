using System.Collections.Generic;
using Godot;

public interface E_IEnemyComponent
{
    public NodePath EnemyPath   {get; set;}
    public E_IEnemy? Enemy      {get; set;}

    public void SetEnemy(Node owner, ref NodePath pathProperty, NodePath value)
    {
        if (pathProperty == value)
            return;

        pathProperty = value;
        ResolveEnemy(owner);
        owner.UpdateConfigurationWarnings();
    }

    public bool ResolveEnemy(Node node)
    {
        Enemy = null;
        if (EnemyPath == null || !node.IsInsideTree())
            return false;

        var enemyNode = node.GetNodeOrNull(EnemyPath);

        if (enemyNode is not E_IEnemy validEnemy)
            return false;

        Enemy = validEnemy;
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