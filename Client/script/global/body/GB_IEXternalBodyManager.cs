using Godot;

public interface GB_IExternalBodyManager
{
    void ResetVelocity(Vector3 velocity);
    void HandleKnockBack(Vector3 force);
    Vector3 Velocity();
    Vector3 PrevVelocity {get;}
    Transform3D GlobalTransform {get;}
    Transform3D PrevGlobalTransform {get;}
}