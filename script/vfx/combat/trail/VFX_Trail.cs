using Godot;

namespace Pew;

[GlobalClass]
public abstract partial class VFX_Trail : Resource
{
    public abstract void Shoot(Node manager, Vector3 origin, Vector3 hit);
}