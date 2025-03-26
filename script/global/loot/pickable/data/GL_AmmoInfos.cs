using Godot;

[GlobalClass]
public partial class GL_AmmoInfos : GL_PickableInfos
{
    [Export] private uint _weaponIndex; // 0 to split between weapons
    [Export] private uint _fireIndex;   // 0 to split between fires
    [Export] private uint _ammos;

    protected override GL_PhysicsPickable GeneratePhysicsPickable() => new GL_PhysicsPickable(new GL_AmmoPickHandler(_weaponIndex, _fireIndex, _ammos));
}