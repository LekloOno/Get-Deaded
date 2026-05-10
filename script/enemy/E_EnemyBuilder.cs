using Godot;

[GlobalClass]
public partial class E_EnemyBuilder : Resource
{
    [Export] private PackedScene _enemyBase;
    [Export] private PackedScene _fire;
    [Export] private GCD_Health _health;
    [Export] private PROTO_MoverData _moverData;

    public E_Enemy Build()
    {
        E_Enemy enemy = _enemyBase.Instantiate<E_Enemy>();

        // Health
        GC_Health healthTree = _health.BuildNode();
        enemy.HealthManager.TopHealthLayer = healthTree;

        for (GC_Health node = healthTree; node != null; node = node.Child)
            enemy.HealthManager.AddChild(node);

        // Mover
        PROTO_Mover mover = new(_moverData, enemy);
        enemy.MoverWrapper.Mover = mover;
        enemy.Mover = mover;

        enemy.AddChild(mover);

        // Fire
        PW_FireBis fire = _fire.Instantiate<PW_FireBis>();
        enemy.Fire = fire;

        enemy.AimPosition.AddChild(fire);


        return enemy;
    }
}