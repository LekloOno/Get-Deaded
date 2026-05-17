using Godot;

[GlobalClass]
public abstract partial class VFX_Trail : Resource, VFX_ITrail
{
    public abstract void Shoot(Node manager, Vector3 origin, Vector3 hit);
    public abstract void Preload(Node manager, uint count);
}