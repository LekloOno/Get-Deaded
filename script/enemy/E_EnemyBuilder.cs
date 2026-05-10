using Godot;

[GlobalClass]
public partial class E_EnemyBuilder : Resource
{
    [Export] private PackedScene _enemyBase;
    [Export] private PackedScene _fire;
    [Export] private GCD_Health _health;
    [Export] private PROTO_MoverData _moverData;
    [Export] private uint _score = 50;
    [Export] private float _speedSpreadFactor = 15f;
    [Export] private double _reactionTime = 0.2f;

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

        // Stats
        enemy.Score = _score;
        enemy.SpeedSpreadFactor = _speedSpreadFactor;
        enemy.ReactionTime = _reactionTime;


        return enemy;
    }
}