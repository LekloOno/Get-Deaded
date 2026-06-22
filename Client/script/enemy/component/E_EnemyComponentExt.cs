using Godot;

public static class EnemyComponentUtil
{
    public static void SetEnemy(this E_IEnemyComponent comp, Node owner, ref NodePath pathProperty, NodePath value)
    {
        if (pathProperty == value)
            return;

        pathProperty = value;
        comp.ResolveEnemy(owner);
        owner.UpdateConfigurationWarnings();
    }
}