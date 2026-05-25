using System.Collections.Generic;
using Godot;

[GlobalClass]
public abstract partial class VFX_Trail : Resource, VFX_ITrail
{
    public VFX_Trail()
    {
        VFX_TrailPoolLoader.Register(this);
    }

    [Export] private uint _preloadCount = 5;
    public abstract void Shoot(Vector3 origin, Vector3 hit);

    protected Stack<VFX_ITrailMesh> _meshesPool = new();

    private bool _preloaded = false;

    public void Preload(Node manager)
    {
        if (_preloaded)
            return;
        
        _preloaded = true;

        for (int i = 0; i < _preloadCount; i++)
        {
            VFX_TrailMesh newMesh = CreateTrail();
            _meshesPool.Push(newMesh);
            manager.AddChild(newMesh);
            newMesh.Pool();
            newMesh.Pooled += _meshesPool.Push;
        }
    }

    public abstract VFX_TrailMesh CreateTrail();
}