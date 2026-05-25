using Godot;

public interface VFX_ITrail
{
    void Shoot(Vector3 origin, Vector3 hit);
    void Preload(Node manager);
}