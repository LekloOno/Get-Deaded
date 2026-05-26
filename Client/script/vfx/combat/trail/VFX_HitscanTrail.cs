using Godot;

[GlobalClass]
public abstract partial class VFX_HitscanTrail : VFX_Trail
{
    public override sealed void Shoot(Vector3 origin, Vector3 hit)
    {
        
        if (_meshesPool.TryPop(out VFX_ITrailMesh? mesh) &&
            mesh != null &&
            mesh is Node meshNode)
        {
            mesh.Shoot(origin, hit);
            return;
        } else
        {
            VFX_TrailMesh newMesh = CreateTrail();
            newMesh.Pooled += Pool;
            if (VFX_TrailPoolLoader.Instance == null)
                return;
            
            VFX_TrailPoolLoader.Instance.AddChild(newMesh);
            newMesh.Shoot(origin, hit);
        }

    }

    private void Pool(VFX_ITrailMesh _mesh)
    {
        _meshesPool.Push(_mesh);
    }
}