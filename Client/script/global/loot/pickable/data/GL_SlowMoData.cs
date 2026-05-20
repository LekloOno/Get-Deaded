using Godot;

[GlobalClass]
public partial class GL_SlowMoData : GL_PickableData
{
    [Export] public PackedScene? SlowMoProcess;
    [Export] public float Factor {get; private set;}
    [Export] public float Duration {get; private set;}

    protected override GL_PhysicsPickable GetPhysicsPickable(float horizontalDamp, float lifeTime) => new GL_PhysicsPickable(new GL_SlowMoPickHandler((GL_SlowMoData) Duplicate()), horizontalDamp, lifeTime);
}