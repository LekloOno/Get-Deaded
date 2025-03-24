using System;
using Godot;

public abstract class PCR_BaseHandler
{
    public bool Sleep;
    public EventHandler Completed;

    public PCR_BaseHandler(Vector2 angle, float time, bool sleep = false)
    {
        Sleep = sleep;
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