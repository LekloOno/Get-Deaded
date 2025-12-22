using Godot;

namespace Pew;

[GlobalClass]
public abstract partial class VFX_HitscanTrail : VFX_Trail
{
    [Export] protected Material _material;
    protected VFX_TrailMesh _mesh;
    
    public override sealed void Shoot(Node manager, Vector3 origin, Vector3 hit)
    {
        _mesh = CreateTrail(origin, hit, (Material)_material.Duplicate());
        _mesh.TopLevel = true;
        manager.AddChild(_mesh);
        _mesh.Shoot();
    }

    public abstract VFX_TrailMesh CreateTrail(Vector3 origin, Vector3 hit, Material material);
}