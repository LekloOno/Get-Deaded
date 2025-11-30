using Godot;

[GlobalClass]
public abstract partial class GB_ExternalBodyManagerWrapper : Node, GB_IExternalBodyManager
{
    public abstract GB_IExternalBodyManager Body {get;}
    public void HandleKnockBack(Vector3 force)
    {
        Body.HandleKnockBack(force);
    }

    public Vector3 Velocity() =>
        Body.Velocity();
}