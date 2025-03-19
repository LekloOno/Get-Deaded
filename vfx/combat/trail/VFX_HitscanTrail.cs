using Godot;

[GlobalClass]
public abstract partial class VFX_HitscanTrail : Resource
{
    [Export] protected Material _material;
    protected VFX_TrailMesh _mesh;
    
    public void Shoot(Node manager, Vector3 origin, Vector3 hit)
    {
        _mesh = CreateTrail(origin, hit, _material);
        _mesh.MaterialOverride = _material;
        _mesh.TopLevel = true;
        manager.AddChild(_mesh);
        _mesh.Shoot();
    }

    public abstract VFX_TrailMesh CreateTrail(Vector3 origin, Vector3 hit, Material material);
}