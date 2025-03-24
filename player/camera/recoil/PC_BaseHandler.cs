using System;
using Godot;

public abstract class PC_BaseHandler
{
    protected Vector2 _velocity = Vector2.Zero;
    protected Vector2 _resistance = Vector2.Zero;
    public EventHandler Completed;

    public PC_BaseHandler(Vector2 angle, float time)
    {
        float resX = 2f*angle.X/Mathf.Pow(time, 2f);
        float resY = 2f*angle.Y/Mathf.Pow(time, 2f);
        _resistance = new(resX, resY);
    }

    public void AddVelocity(Vector2 velocity) => _velocity += velocity;
    public void AddCappedVelocity(Vector2 velocity, float max)
    {
        AddVelocity(velocity);
        if (_velocity.Length() > max)
            _velocity = _velocity.Normalized() * max;
    }

    public void AddAngle(Vector2 angle)
    {
        float radAngleX = Mathf.DegToRad(angle.X);
        float radAngleY = Mathf.DegToRad(angle.Y);

        float velocityX = Mathf.Sqrt(2f*_resistance.X*radAngleX);
        float velocityY = Mathf.Sqrt(2f*_resistance.Y*radAngleY);

        _velocity += new Vector2(velocityX, velocityY);
    }

    public abstract bool Tick(double delta, out Vector2 tickVelocity);
}