using Godot;

[GlobalClass]
public partial class VFX_LineTrail : VFX_HitscanTrail
{
    [Export] private float _fadeTime = 0.5f;

    public override VFX_TrailMesh CreateTrail(Vector3 origin, Vector3 hit, Material material) =>
        new VFX_LineTrailMesh(origin, hit, material, _fadeTime);
}