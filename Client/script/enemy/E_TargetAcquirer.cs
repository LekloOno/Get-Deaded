using Godot;

public partial class E_TargetAcquirer : Area3D
{
    private static uint Count = 0;
    private readonly uint _id = Count ++;
    private const uint TicksMask = 63;

    public GE_ICombatEntity? Target { get; private set; }

    public override void _Ready()
    {
        CollisionMask = CONF_Collision.Layers.PlayerHurtBox;
    }

    public override void _PhysicsProcess(double delta)
    {
        if ((Engine.GetPhysicsFrames() & TicksMask) != (_id & TicksMask))
            return;

        Target = GetTarget();
    }

    private GE_CombatEntity? GetTarget()
    {
        var bodies = GetOverlappingAreas();

        foreach (Node3D node in bodies)
        {
            if (node is not GC_HurtBox hurtBox)
                continue;
            
            return hurtBox.Entity;
        }

        return null;
    }
}