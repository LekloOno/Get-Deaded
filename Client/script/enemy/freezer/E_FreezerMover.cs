using System;
using Godot;

public partial class E_FreezerMover : Node
{
    [Export] private DATA_FreezerMover  _data = null!;
    [Export] private CharacterBody3D    _body = null!;
    [Export] private RayCast3D          _groundCast = null!;

    public override void _Ready()
    {
        _body.MotionMode = CharacterBody3D.MotionModeEnum.Floating;
    }

    private double _elapsedStraffe = 0f;
    private double _elapsedFloat = 0f;
    private bool _left;
    private bool _up;
    public override void _PhysicsProcess(double delta)
    {
        SetStraffe(delta);
        SetFloat(delta);

        Vector3 velocity = _body.GetRealVelocity();
        
        velocity.X /= 1f + _data.Drag;
        velocity.Z /= 1f + _data.Drag;

        velocity = Accelerate(velocity);

        _body.Velocity = velocity;
        _body.MoveAndSlide();
    }

    private readonly Random _rng = new();
    private void SetStraffe(double delta)
    {
        _elapsedStraffe -= delta;
        if (_elapsedStraffe > 0f)
            return;

        _left ^= true;

        float seed = _rng.NextSingle();
		_elapsedStraffe = Mathf.Lerp(_data.MinStraffe, _data.MaxStraffe, seed);
    }

    private void SetFloat(double delta)
    {
        _elapsedFloat -= delta;

        if (!GoingTooHigh() && _elapsedFloat > 0f)
            return;

        _up ^= true;

        float seed = _rng.NextSingle();
		_elapsedFloat = Mathf.Lerp(_data.MinFloat, _data.MaxFloat, seed);
    }

    private bool GoingTooHigh() =>
        _up && GetHeight() >= Mathf.Abs(_groundCast.TargetPosition.Y);

    private float GetHeight()
    {
        if (!_groundCast.IsColliding())
            return Mathf.Abs(_groundCast.TargetPosition.Y);

        Vector3 pos = _groundCast.GetCollisionPoint();
        return _groundCast.GlobalPosition.DistanceTo(pos);
    }

    private Vector3 GetStraffeDir() => _left ? _body.GlobalTransform.Basis.X : -_body.GlobalTransform.Basis.X;
    private Vector3 GetFloatDir()   => _up   ? Vector3.Up   : Vector3.Down;
    private Vector3 Accelerate(Vector3 velocity) =>
        HorizontalAccelerate(velocity) +
        VerticalAccelerate(velocity);

    private Vector3 HorizontalAccelerate(Vector3 velocity)
    {
        velocity.Y = 0;

        Vector3 acceleration = GetStraffeDir() * _data.Acceleration;
        velocity += acceleration;

        float speed = Mathf.Min(velocity.Length(), _data.MaxSpeed);
        return velocity.Normalized() * speed;
    }

    private Vector3 VerticalAccelerate(Vector3 velocity)
    {
        velocity.X = velocity.Z = 0;

        Vector3 acceleration = GetFloatDir() * _data.FloatAcceleration;
        velocity += acceleration;

        float speed = Mathf.Min(Mathf.Abs(velocity.Y), _data.FloatMaxSpeed);
        return velocity.Normalized() * speed;
    }
}