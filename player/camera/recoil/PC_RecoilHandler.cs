using System;
using Godot;

public class PC_RecoilHandler
{
    private Vector2 _velocity = Vector2.Zero;
    private Vector2 _resistance = Vector2.Zero;
    public bool _autoRemove;
    private float _threshold;

    public EventHandler Completed;

    public PC_RecoilHandler(Vector2 angle, float time, float threshold, bool autoRemove)
    {
        _autoRemove = autoRemove;
        _threshold = threshold;

        float radAngleX = Mathf.DegToRad(angle.X);
        float radAngleY = Mathf.DegToRad(angle.Y);

        float resX = 2f*radAngleX/Mathf.Pow(time, 2f);
        float resY = 2f*radAngleY/Mathf.Pow(time, 2f);
        _resistance = new(resX, resY);

        _velocity = _resistance * time;
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

    public bool Tick(double delta, out Vector2 tickVelocity)
    {
        tickVelocity = _velocity * (float) delta;
        return _autoRemove && ApplyResistance(delta);
    }

    public void TickReset(double delta, out Vector2 tickVelocity)
    {
        tickVelocity = _velocity * (float) delta;
        _velocity -= _resistance * (float)delta;
    }

    protected bool ApplyResistance(double delta)
    {
        Vector2 appliedRes = _resistance * (float)delta;
        bool ended = appliedRes.Length() > _velocity.Length();
        if (ended)
        {
            _velocity = Vector2.Zero;
            Completed.Invoke(this, EventArgs.Empty);
        }
        else
            _velocity -= appliedRes;

        return ended;
    }
}