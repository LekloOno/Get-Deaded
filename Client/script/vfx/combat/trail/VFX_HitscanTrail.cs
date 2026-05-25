using Godot;

[GlobalClass]
public abstract partial class VFX_HitscanTrail : VFX_Trail
{
    //protected Stack<VFX_ITrailMesh> _meshesPool = new();

    private bool _preloaded = false;

    public override void Preload(Node manager, uint count)
    {
        if (_preloaded)
            return;
        
        _preloaded = true;
        VFX_TrailMesh newMesh = CreateTrail();
        manager.AddChild(newMesh);
        manager.RemoveChild(newMesh);
        /*
        if (_meshesPool.Count != 0)
            return;

        for (int i = 0; i < count; i++)
        {
            VFX_TrailMesh newMesh = CreateTrail((Material)_material.Duplicate());
            newMesh.Pooled += _meshesPool.Push;
            manager.AddChild(newMesh);
            _meshesPool.Push(newMesh);
        }*/
    }
    
    public override sealed void Shoot(Node manager, Vector3 origin, Vector3 hit)
    {
        /*
        if (_meshesPool.TryPop(out VFX_ITrailMesh mesh))
        {
            mesh.Shoot(origin, hit);
            return;
        }*/

        VFX_TrailMesh newMesh = CreateTrail();
        //newMesh.Pooled += _meshesPool.Push;
        newMesh.Pooled += (_) => newMesh.QueueFree();
        manager.AddChild(newMesh);
        newMesh.Shoot(origin, hit);
    }

    public abstract VFX_TrailMesh CreateTrail();
}