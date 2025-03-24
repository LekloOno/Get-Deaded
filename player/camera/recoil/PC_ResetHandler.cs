using System;
using Godot;

public class PC_ResetHandler : PC_BaseHandler
{
    private Vector2 _targetAngle;
    private Vector2 _angle;
    private float _time;
    private Vector2 _angleCubeTime;
    private float _x;

    public PC_ResetHandler(Vector2 angle, float time) : base(angle, time)
    {
        _targetAngle = angle;
        _angle = Vector2.Zero;
        _velocity = Vector2.Zero;
        _time = time;
        _angleCubeTime = 6f*angle / Mathf.Pow(time, 3f);
    }

    protected override bool DoTick(double delta, out Vector2 tickVelocity)
    {
        _x += (float) delta;
        tickVelocity = -EaseInOutAnim() * (float) delta;
        GD.Print(tickVelocity);
        return _x >= _time;
    }

    private Vector2 EaseInOutAnim() => _angleCubeTime * _x * (_time - _x);
}