using System;
using Godot;

public delegate void ActionInputEvent<T>(object sender, T args);

public abstract partial class PI_Action<T> : Node
{    
    public ActionInputEvent<T> OnStartInput;
    public ActionInputEvent<T> OnStopInput;
    public ActionInputEvent<T> OnInputPerformed;
    public bool IsDown {get; protected set;}
    // Input is currently up or down. Not strictly related to the active state.
    //      e.g. Input is down, but it is a Tap type of input, the last input deactivated the active state.
    /*
    public bool IsOn {get; protected set;}
    // Input is currently active or not. Not strictly related to the action state.
    //      e.g. Input is active but further behavior of the input handler didn't recognize a valid state for the related action.
    */

    protected abstract ACTIONS_Action Action {get;}
    

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event.IsActionPressed(Action))
        {
            IsDown = true;
            Send(InputDown(@event, out T value), value);
        }
        else if (@event.IsActionReleased(Action))
        {
            IsDown = false;
            Send(InputUp(@event, out T value), value);
        }
    }

    private void Send(PI_ActionState actionState, T value)
    {
        switch (actionState)
        {
            case PI_ActionState.STARTED:
                OnStartInput?.Invoke(this, value);
                break;
            case PI_ActionState.STOPPED:
                OnStopInput?.Invoke(this, value);
                break;
            case PI_ActionState.PERFORMED:
                OnInputPerformed?.Invoke(this, value);
                break;
            default:
                break;
        }
    }

    public abstract PI_ActionState InputDown(InputEvent @event, out T value);
    public abstract PI_ActionState InputUp(InputEvent @event, out T value);
}