using System;
using Godot;

public class PC_ResetHandler
{
    private Vector2 _velocity = Vector2.Zero;
    private Vector2 _resistance = Vector2.Zero;
    public bool _autoRemove;
    private Vector2 _targetAngle;
    private Vector2 _angle;

    public EventHandler Completed;

    public PC_ResetHandler(Vector2 angle, float time)
    {
        _targetAngle = new(angle.X, angle.Y);
        _angle = Vector2.Zero;

        float resX = 2f*angle.X/Mathf.Pow(time, 2f);
        float resY = 2f*angle.Y/Mathf.Pow(time, 2f);
        _resistance = new(resX, resY);

        GD.Print(_resistance);

        _velocity = Vector2.Zero;
    }

    public bool Tick(double delta, out Vector2 tickVelocity)
    {
        _velocity -= _resistance * (float)delta;
        tickVelocity = _velocity * (float) delta;
        _angle += tickVelocity;
        GD.Print(_angle.Length() + " >= " + _targetAngle.Length());
        return _angle.Length() >= _targetAngle.Length();
    }
}