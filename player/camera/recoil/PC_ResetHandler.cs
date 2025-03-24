using System;
using Godot;

public class PC_ResetHandler : PC_BaseHandler
{
    private Vector2 _targetAngle;
    private Vector2 _angle;

    public PC_ResetHandler(Vector2 angle, float time) : base(angle, time)
    {
        _targetAngle = new(angle.X, angle.Y);
        _angle = Vector2.Zero;
        _velocity = Vector2.Zero;
    }

    protected override bool DoTick(double delta, out Vector2 tickVelocity)
    {
        _velocity -= _resistance * (float)delta;
        tickVelocity = _velocity * (float) delta;
        _angle += tickVelocity;
        return _angle.Length() >= _targetAngle.Length();
    }
}