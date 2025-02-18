using System;
using Godot;

public interface PI_Holdable
{
    bool IsActive {get;}
    bool Hold {get;}
    string Action {get;}
    
    protected void StartAction();
    protected void StopAction();

    protected void Handle(InputEvent @event)
    {
        if (Hold)
            HandleHold(@event);
        else
            HandleSimple(@event);
    }

    protected void HandleHold(InputEvent @event)
    {
        if (@event.IsActionPressed(Action) && !@event.IsEcho())
            StartAction();
        else if(@event.IsActionReleased(Action))
            StopAction();
    }

    protected void HandleSimple(InputEvent @event)
    {
        if (@event.IsActionPressed(Action) && !@event.IsEcho())
        {
            if (IsActive)
                StopAction();
            else
                StartAction();
        }
    }
}