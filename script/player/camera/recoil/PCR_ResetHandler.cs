using System;
using Godot;

public class PCR_ResetHandler : PCR_BaseHandler
{
    private readonly float _time;
    private Vector2 _angleCubeTime;
    private float _x;

    public PCR_ResetHandler(Vector2 angle, float time) : base(angle, time)
    {
        _time = time;
        _angleCubeTime = 6f*angle / Mathf.Pow(time, 3f);
        _x = 0f;
    }

    protected override bool DoTick(double delta, out Vector2 tickVelocity)
    {
        _x += (float) delta;
        tickVelocity = -EaseInOutAnim() * (float) delta;

        bool completed = _x >= _time;
        if (completed)
            Completed?.Invoke(this, EventArgs.Empty);

        return completed;
    }

    // Derivative of a simple cubic smoothstep function.
    // https://www.desmos.com/calculator/qmlv71zgtr?lang=fr
    private Vector2 EaseInOutAnim() => _angleCubeTime * _x * (_time - _x);
}