using System;
using Godot;

public abstract class PC_BaseHandler
{
    protected Vector2 _velocity = Vector2.Zero;
    protected Vector2 _resistance = Vector2.Zero;
    public bool Sleep;
    public EventHandler Completed;

    public PC_BaseHandler(Vector2 angle, float time, bool sleep = false)
    {
        ComputeResistance(angle, time);
        Sleep = sleep;
    }

    public void ComputeResistance(Vector2 angle, float time)
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
        float velocityX = Mathf.Sqrt(2f*_resistance.X*angle.X);
        float velocityY = Mathf.Sqrt(2f*_resistance.Y*angle.Y);

        _velocity += new Vector2(velocityX, velocityY);
    }

    public void Reset()
    {
        _velocity = Vector2.Zero;
        Sleep = true;
    }

    public bool Tick(double delta, out Vector2 tickVelocity)
    {
        if(Sleep)
        {
            tickVelocity = Vector2.Zero;
            return false;
        }
        
        return DoTick(delta, out tickVelocity);
    }
    protected abstract bool DoTick(double delta, out Vector2 tickVelocity);
}