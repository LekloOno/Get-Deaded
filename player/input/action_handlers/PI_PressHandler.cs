using Godot;

public abstract partial class PI_PressHandler<T> : PI_ActionHandler<T>
{
    public bool IsDown {get; protected set;}

    // The ActionState resulting of a given pressed down/up InputEvent and value.
    //  -   STARTED, STOPPED, PERFORMED or NONE.
    protected abstract PI_ActionState InputDown(InputEvent @event, out T value);
    protected abstract PI_ActionState InputUp(InputEvent @event, out T value);

    protected override void HandleExternal(PI_ActionState actionState, T value) => Send(actionState, value);
    protected override void HandleInput(InputEvent @event)
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
}