using Godot;

[GlobalClass]
public abstract partial class VFX_LineType : Resource
{
    public abstract ImmediateMesh GenerateMesh(Vector3 origin, Vector3 direction, Camera3D camera);
}