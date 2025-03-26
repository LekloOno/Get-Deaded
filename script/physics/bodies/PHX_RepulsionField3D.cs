using Godot;

[GlobalClass]
public partial class PHX_RepulsionField3D : ShapeCast3D
{
    [Export] private PHX_RepulsionField3DData _data;
    private RigidBody3D _bounceBody;

    public PHX_RepulsionField3D(){}
    public PHX_RepulsionField3D(PHX_RepulsionField3DData data)
    {
        _data = data;
    }

    public override void _Ready()
    {
        GD.Print("oi");
        if (GetParent() is RigidBody3D body)
        {
            _bounceBody = body;
            Shape = _data.SphereShape;
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
            float forceRatio = (_data.SphereShape.Radius - direction.Length()) / _data.SphereShape.Radius;
            float force = _data.RepulsingCurve.Sample(forceRatio);
            _bounceBody.ApplyForce(force * direction.Normalized() * _data.RepulsingStrength);
        }
    }
}