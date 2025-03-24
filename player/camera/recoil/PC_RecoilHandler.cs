using System;
using Godot;

public class PC_RecoilHandler : PC_BaseHandler
{
    public bool _autoRemove;

    public PC_RecoilHandler(Vector2 angle, float time, bool autoRemove, bool sleep = false) : base(angle, time, sleep)
    {
        _autoRemove = autoRemove;
        _velocity = _resistance * time;
    }

    public static PC_RecoilHandler InitRecoil(Vector2 angle, float time)
    {
        PC_RecoilHandler handler = new(angle, time, false);
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