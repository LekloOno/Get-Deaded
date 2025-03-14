using Godot;

public abstract partial class PI_HoldableHandler<T> : PI_PressHandler<T>
{
    [Export] public bool Hold = false;
    
    protected bool _active = false;

    // Allow to reset state from an external source without casting Stop event.
    //  -   e.g. for transitions.
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

    protected override void HandleExternal(PI_ActionState actionState, T value)
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