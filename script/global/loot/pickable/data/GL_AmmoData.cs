using Godot;

[GlobalClass]
public partial class GL_AmmoData : GL_PickableData
{
    [Export] public int WeaponIndex {get; private set;}    // 0 to split between weapons, -1 for currently active weapon
    [Export] public int FireIndex {get; private set;}      // 0 to split between fires
    [Export] public int Ammos {get; private set;}

    protected override GL_PhysicsPickable GetPhysicsPickable(float horizontalDamp, float lifeTime) => new GL_PhysicsPickable(new GL_AmmoPickHandler((GL_AmmoData) Duplicate()), horizontalDamp, lifeTime);
}