using Godot;

public partial class GB_CharacterBody : CharacterBody3D, GB_IExternalBodyManager
{
    public void HandleKnockBack(Vector3 force)
    {
        Velocity += force;
    }

    public void ResetVelocity(Vector3 velocity)
    {
        Velocity = velocity;
    }

    Vector3 GB_IExternalBodyManager.Velocity() => Velocity;
    public Vector3 PrevVelocity {get; protected set;}

    Transform3D GB_IExternalBodyManager.GlobalTransform => GlobalTransform;
    public Transform3D PrevGlobalTransform {get; protected set;}

    public override sealed void _PhysicsProcess(double delta)
    {
        PrevGlobalTransform = GlobalTransform;
        PrevVelocity = PrevVelocity;
        PhysicsProcessSpec(delta);
    }

    protected virtual void PhysicsProcessSpec(double delta) {}

    public void Teleport(Vector3 position)
    {
        GlobalPosition = position;
        ResetPhysicsInterpolation();
    }

}