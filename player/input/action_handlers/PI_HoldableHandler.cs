using Godot;

public abstract partial class PI_HoldableHandler<T> : PI_PressHandler<T>
{
    [Export] public bool Hold = false;
    
    private bool _active = false;

    protected override PI_ActionState InputDown(InputEvent @event, out T value)
    {
        value = GetInputValue(@event);

        if (Hold)
            return PI_ActionState.STARTED;

        if (_active)
            return PI_ActionState.STOPPED;
        return PI_ActionState.STARTED;
    }
    protected override PI_ActionState InputUp(InputEvent @event, out T value)
    {
        value = GetInputValue(@event);

        if (Hold)
            return PI_ActionState.STOPPED;
        return PI_ActionState.NONE;
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
}