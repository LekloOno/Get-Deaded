using Godot;

namespace Pew;

public abstract partial class GB_CharacterBody : CharacterBody3D, GB_IExternalBodyManager
{
    public void HandleKnockBack(Vector3 force)
    {
        Velocity += force;
    }

    Vector3 GB_IExternalBodyManager.Velocity() => Velocity;
}