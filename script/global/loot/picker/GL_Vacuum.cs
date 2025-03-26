using Godot;

[GlobalClass]
public partial class GL_Vacuum : Area3D
{
    [Export] private Curve _vacuumCurve;
    [Export] private float _vacuumStrength;
    private SphereShape3D _sphere;
    public override void _Ready()
    {
        CollisionShape3D collisionShape = GetChild<CollisionShape3D>(0);
        if (collisionShape?.Shape is SphereShape3D sphere)
        {
            _sphere = sphere;
            Enable();
        }
        else
            Disable();
    }

    public override void _PhysicsProcess(double delta)
    {
        foreach (Node3D body in GetOverlappingBodies())
        {
            if (body is RigidBody3D rigidBody)
            {
                GD.Print("oui");
                Vector3 direction = GlobalPosition - rigidBody.GlobalPosition;
                float forceRatio = (_sphere.Radius - direction.Length()) / _sphere.Radius;
                float force = _vacuumCurve.Sample(forceRatio);
                rigidBody.ApplyForce(force * direction.Normalized() * _vacuumStrength);
            }
        }
    }

    public void Disable()
    {
        CollisionMask = 0;
        SetPhysicsProcess(false);
    }

    public void Enable()
    {
        CollisionMask = 0x10;
        SetPhysicsProcess(true);
    }
}