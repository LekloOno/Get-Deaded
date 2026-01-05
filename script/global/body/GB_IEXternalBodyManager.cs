using Godot;

public interface GB_IExternalBodyManager
{
    void HandleKnockBack(Vector3 force);
    Vector3 Velocity();
    Transform3D GlobalTransform {get;}
}