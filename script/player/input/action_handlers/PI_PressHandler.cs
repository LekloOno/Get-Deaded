using Godot;

namespace Pew;

public abstract partial class PI_PressHandler<T> : PI_ActionHandler<T>
{
    protected abstract ACTIONS_Action Action {get;}
    public bool IsDown {get; protected set;}

    /// <summary>
    /// Called whenever the input is pressed Down.
    /// It should handle the input, such as casting corresponding event given the current handler state.
    /// </summary>
    /// <param name="event">The received event. STARTED, STOPPED, PERFORMED or NONE.</param>
    protected abstract void InputDown(InputEvent @event);
    /// <summary>
    /// Called whenever the input is pressed Up.
    /// It should handle the input, such as casting corresponding event given the current handler state.
    /// </summary>
    /// <param name="event">The received event. STARTED, STOPPED, PERFORMED or NONE.</param>
    protected abstract void InputUp(InputEvent @event);

    public override void HandleExternal(PI_ActionState actionState, T value) => Send(actionState, value);
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