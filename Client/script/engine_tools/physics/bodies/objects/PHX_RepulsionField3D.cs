using System.Collections.Generic;
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
        if (GetParent() is RigidBody3D body)
        {
            _bounceBody = body;
            Shape = _data.SphereShape;
            CollisionMask = _data.CollisionMask;
            TargetPosition = Vector3.Zero;
        }
        else
            SetPhysicsProcess(false);
        
    }
    public override void _PhysicsProcess(double delta)
    {
        if (!IsColliding())
            return;
        
        Vector3 force = Vector3.Zero;
        for (int i = 0; i < GetCollisionCount(); i++)
        {
            Vector3 point  = GetCollisionPoint(i);
            Vector3 normal = GetCollisionNormal(i);

            float depth = _data.SphereShape.Radius - (GlobalPosition - point).Dot(normal);
            depth = Mathf.Max(depth, 0f);

            float forceRatio = depth / _data.SphereShape.Radius;
            float strength = _data.RepulsingCurve.Sample(forceRatio);

            force += strength * normal * _data.RepulsingStrength;
        }

        _bounceBody.ApplyForce(force);
    }
}