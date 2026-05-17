using System;
using Godot;

public abstract partial class VFX_TrailMesh : MeshInstance3D, VFX_ITrailMesh
{
    public Vector3 Origin {get; protected set;}
    public Vector3 Hit {get; protected set;}

    public VFX_TrailMesh(Material material)
    {
        TopLevel = true;
        MaterialOverride = material;
    }

    public event Action<VFX_ITrailMesh> Pooled;

    public void Shoot(Vector3 origin, Vector3 hit)
    {
        Origin = origin;
        Hit = hit;
        Spawn();
        SpecShoot(origin, hit);
    }

    protected abstract void SpecShoot(Vector3 origin, Vector3 hit);
    public void Pool()
    {
        SpecPool();
        Pooled?.Invoke(this);
    }
    protected abstract void SpecPool();
    public abstract void Spawn();
}