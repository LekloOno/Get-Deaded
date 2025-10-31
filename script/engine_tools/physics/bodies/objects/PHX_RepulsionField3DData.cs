using Godot;

[GlobalClass]
public partial class PHX_RepulsionField3DData : Resource
{
    [Export] public float RepulsingStrength {get; private set;}
    [Export] public Curve RepulsingCurve {get; private set;}
    [Export] public SphereShape3D SphereShape {get; private set;}
    [Export(PropertyHint.Layers3DPhysics)] public uint CollisionMask {get; private set;}
}