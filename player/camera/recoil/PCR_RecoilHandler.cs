using System;
using Godot;

public class PCR_RecoilHandler : PCR_BaseHandler
{
    private Vector2 _velocity = Vector2.Zero;
    private Vector2 _resistance = Vector2.Zero;
    public bool _autoRemove;

    public PCR_RecoilHandler(Vector2 angle, float time, bool autoRemove, bool sleep = false) : base(angle, time, sleep)
    {
        ComputeResistance(angle, time);
        _autoRemove = autoRemove;
        _velocity = _resistance * time;
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

    public static PCR_RecoilHandler InitRecoil(Vector2 angle, float time)
    {
        PCR_RecoilHandler handler = new(angle, time, false);
        handler.Reset();
        return handler;
    }

    protected override bool DoTick(double delta, out Vector2 tickVelocity)
    {
        tickVelocity = _velocity * (float) delta;
        bool stopped = ApplyResistance(delta);
        return _autoRemove && stopped;
    }

    protected bool ApplyResistance(double delta)
    {
        Vector2 appliedRes = _resistance * (float)delta;
        bool ended = appliedRes.Length() > _velocity.Length();
        if (ended)
        {
            _velocity = Vector2.Zero;
            Completed?.Invoke(this, EventArgs.Empty);
        }
        else
            _velocity -= appliedRes;

        return ended;
    }
}