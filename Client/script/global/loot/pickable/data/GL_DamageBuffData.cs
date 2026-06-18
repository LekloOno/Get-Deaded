using Godot;

[GlobalClass]
public partial class GL_DamageBuffData : GL_PickableData
{
    [Export] public float Multiplier {get; private set;}
    [Export] public float Duration {get; private set;}

    protected override GL_PhysicsPickable GetPhysicsPickable(float horizontalDamp, float lifeTime) => new (new GL_DamageBuffPickHandler((GL_DamageBuffData) Duplicate()), horizontalDamp, lifeTime);
}