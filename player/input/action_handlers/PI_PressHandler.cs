using Godot;

public abstract partial class PI_PressHandler<T> : PI_ActionHandler<T>
{
    public bool IsDown {get; protected set;}
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

    protected abstract PI_ActionState InputDown(InputEvent @event, out T value);
    protected abstract PI_ActionState InputUp(InputEvent @event, out T value);
    protected abstract T GetInputValue(InputEvent @event);
}