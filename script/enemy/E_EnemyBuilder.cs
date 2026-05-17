using Godot;

[GlobalClass]
public partial class E_EnemyBuilder : Resource
{
    [Export] private E_EnemySettings _settings;
    [Export] private PackedScene _enemyBase;

    public E_Enemy Build()
    {
        E_Enemy enemy = _enemyBase.Instantiate<E_Enemy>();
        enemy.SetSettings(_settings);
        return enemy;
    }
}