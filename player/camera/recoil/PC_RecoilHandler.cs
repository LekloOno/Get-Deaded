using System;
using Godot;

public class PC_RecoilHandler : PC_BaseHandler
{
    public bool _autoRemove;

    public PC_RecoilHandler(Vector2 angle, float time, bool autoRemove) : base(angle, time)
    {
        _autoRemove = autoRemove;
        _velocity = _resistance * time;
    }
    public override bool Tick(double delta, out Vector2 tickVelocity)
    {
        tickVelocity = _velocity * (float) delta;
        return _autoRemove && ApplyResistance(delta);
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