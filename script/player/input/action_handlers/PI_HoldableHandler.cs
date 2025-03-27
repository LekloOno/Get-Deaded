using Godot;

/// <summary>
/// A simple Holdable press input implementation.
/// It can be configured on Tap or Hold mode.
/// On Hold mode, the Start event is called when pressing the input down, and Stop when pressing the input up.
/// On Tap mode, the Start and Stop event are both called in input down. Stop event if the previous state was active, Start otherwise.
/// </summary>
/// <typeparam name="T">The input value type.</typeparam>
public abstract partial class PI_HoldableHandler<T> : PI_PressHandler<T>
{
    [Export] public bool Hold = false;
    
    protected bool _active = false;

    /// <summary>
    /// Allows to reset state from an external source without casting Stop event.
    /// -   e.g. for transitions.
    /// </summary>
    public void Reset() => _active = false;

    protected override void InputDown(InputEvent @event)
    {
        T value = GetInputValue(@event);

        if (Hold)
            Started(value);
        else if (_active)
            Stopped(value);
        else
            Started(value);
    }
    protected override void InputUp(InputEvent @event)
    {
        if (Hold)
            Stopped(GetInputValue(@event));
    }

    public override void HandleExternal(PI_ActionState actionState, T value)
    {
        switch (actionState)
        {
            case PI_ActionState.STARTED:
                if (_active)
                    return;
                _active = true;
                Start?.Invoke(this, value);
                break;
            case PI_ActionState.STOPPED:
                if (!_active)
                    return;
                _active = false;
                Stop?.Invoke(this, value);
                break;
            default:
                break;
        }
    }
    private void Stopped(T value)
    {
        _active = false;
        Stop?.Invoke(this, value);
    }
    private void Started(T value)
    {
        _active = true;
        Start?.Invoke(this, value);
    }
}