using System;
using Godot;

public abstract partial class PI_ActionInput : Node
{
    public EventHandler OnStartInput;
    public EventHandler OnStopInput;
    public bool IsDown {get; protected set;}
    protected abstract PI_Action Action {get;}
    

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event.IsActionPressed(Action))
        {
            IsDown = true;
            InputDown(@event);
            OnStartInput?.Invoke(this, EventArgs.Empty);
        } else if (@event.IsActionReleased(Action))
        {
            IsDown = false;
            InputUp(@event);
            OnStopInput?.Invoke(this, EventArgs.Empty);
        }
    }

    public abstract void InputDown(InputEvent @event);
    public abstract void InputUp(InputEvent @event);
}