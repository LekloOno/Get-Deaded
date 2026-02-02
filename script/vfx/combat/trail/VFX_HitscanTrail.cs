using System.Collections.Generic;
using Godot;

[GlobalClass]
public abstract partial class VFX_HitscanTrail : VFX_Trail
{
    [Export] protected Material _material;
    protected Stack<VFX_ITrailMesh> _meshesPool = new();

    public override void Preload(Node manager, uint count)
    {
        for (int i = 0; i < count; i++)
        {
            VFX_TrailMesh newMesh = CreateTrail((Material)_material.Duplicate());
            newMesh.Pooled += _meshesPool.Push;
            manager.AddChild(newMesh);
            _meshesPool.Push(newMesh);
        }
    }
    
    public override sealed void Shoot(Node manager, Vector3 origin, Vector3 hit)
    {
        if (_meshesPool.TryPop(out VFX_ITrailMesh mesh))
        {
            mesh.Shoot(origin, hit);
            return;
        }

        VFX_TrailMesh newMesh = CreateTrail((Material)_material.Duplicate());
        newMesh.Pooled += _meshesPool.Push;
        manager.AddChild(newMesh);
        newMesh.Shoot(origin, hit);
    }

    public abstract VFX_TrailMesh CreateTrail(Material material);
}