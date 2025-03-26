using Godot;

[GlobalClass]
public partial class PHX_RepulsionField3D : ShapeCast3D
{
    [Export] private float _repulsingStrength;
    [Export] private Curve _repulsingCurve;
    private RigidBody3D _bounceBody;
    private PhysicsShapeQueryParameters3D _query;
    private float _maxDistance;

    public override void _Ready()
    {
        if (GetParent() is RigidBody3D body)
        {
            _bounceBody = body;
            if (Shape is SphereShape3D sphere)
                _maxDistance = sphere.Radius;
        }
        else
            SetPhysicsProcess(false);
    }
    public override void _PhysicsProcess(double delta)
    {
        if (!IsColliding())
            return;
        
        for (int i = 0; i < GetCollisionCount(); i++)
        {
            Vector3 direction = GlobalPosition - GetCollisionPoint(i);
            float forceRatio = (_maxDistance - direction.Length()) / _maxDistance;
            float force = _repulsingCurve.Sample(forceRatio);
            _bounceBody.ApplyForce(force * direction.Normalized() * _repulsingStrength);
        }
    }
}