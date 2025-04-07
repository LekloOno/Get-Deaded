using Godot;

[GlobalClass]
public abstract partial class GB_ExternalBodyManager : Node
{
    public abstract void HandleKnockBack(Vector3 force);
}