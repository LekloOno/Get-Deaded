using Godot;


// Godot doesn't allow to [Export] interfaces, so this is a simple glue wrapper to get around it.
[GlobalClass]
public abstract partial class GB_ExternalBodyManagerWrapper : Node, GB_IExternalBodyManager
{
    public abstract GB_IExternalBodyManager Body {get;}

    public void HandleKnockBack(Vector3 force)
    {
        Body.HandleKnockBack(force);
    }

    public Vector3 Velocity() => Body.Velocity();
    public Vector3 PrevVelocity => Body.PrevVelocity;

    public Transform3D GlobalTransform => Body.GlobalTransform;
    public Transform3D PrevGlobalTransform => Body.GlobalTransform;
}