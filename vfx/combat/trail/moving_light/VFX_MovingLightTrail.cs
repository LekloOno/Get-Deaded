using Godot;

[GlobalClass]
public partial class VFX_MovingLightTrail : VFX_Trail
{
    [Export] private Material _material;
    [Export] private float _thickness;
    [Export] private float _bulletSpeed;
    [Export] private float _trailSpeed;
    [Export] private float _inclination;
    protected VFX_MovingLightObject _trail;
    
    public override sealed void Shoot(Node manager, Vector3 origin, Vector3 hit)
    {
        _trail = new(origin, hit, (Material)_material.Duplicate(), _bulletSpeed, _trailSpeed, _thickness, _inclination);
        manager.AddChild(_trail);
    }
}