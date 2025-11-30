using Godot;

public interface GB_IExternalBodyManager
{
    public void HandleKnockBack(Vector3 force);
    public Vector3 Velocity();
}