using Godot;

[GlobalClass]
public partial class GL_AmmoData : GL_PickableData
{
    [Export] public uint WeaponIndex {get; private set;}    // 0 to split between weapons
    [Export] public uint FireIndex {get; private set;}      // 0 to split between fires
    [Export] public int Ammos {get; private set;}

    protected override GL_PhysicsPickable GetPhysicsPickable(float horizontalDamp) => new GL_PhysicsPickable(new GL_AmmoPickHandler((GL_AmmoData) Duplicate()), horizontalDamp);
}