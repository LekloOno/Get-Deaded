using Godot;

public abstract partial class VFX_TrailMesh : MeshInstance3D
{
    protected Vector3 _origin;
    protected Vector3 _hit;

    public VFX_TrailMesh(Vector3 origin, Vector3 hit, Material material)
    {
        _origin = origin;
        _hit = hit;
        MaterialOverride = material;
    }

    public abstract void Shoot();
}