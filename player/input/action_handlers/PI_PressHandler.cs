using Godot;

public abstract partial class PI_PressHandler<T> : PI_ActionHandler<T>
{
    public bool IsDown {get; protected set;}

    // The ActionState resulting of a given pressed down/up InputEvent and value.
    //  -   STARTED, STOPPED, PERFORMED or NONE.
    protected abstract void InputDown(InputEvent @event);
    protected abstract void InputUp(InputEvent @event);

    protected override void HandleExternal(PI_ActionState actionState, T value) => Send(actionState, value);
    protected override void HandleInput(InputEvent @event)
    {
        if (@event.IsActionPressed(Action))
        {
            IsDown = true;
            InputDown(@event);
        }
        else if (@event.IsActionReleased(Action))
        {
            IsDown = false;
            InputUp(@event);
        }
    }
}