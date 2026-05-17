using Godot;

[GlobalClass]
public partial class VFX_MovingLightTrail : VFX_HitscanTrail
{
    [Export] private float _thickness;
    [Export] private float _bulletSpeed;
    [Export] private float _trailSpeed;
    [Export] private float _inclination;
    protected VFX_MovingLightObject _trail;

    public override VFX_TrailMesh CreateTrail(Material material) =>
        new VFX_MovingLightObject((Material)_material.Duplicate(), _bulletSpeed, _trailSpeed, _thickness, _inclination);
}