using Godot;

public abstract partial class PI_BufferedHandler<T> : PI_PressHandler<T>
{
    [Export] private ulong _bufferWindow;
    private ulong _lastInput = 0;

    public bool UseBuffer()
    {
        bool wasBuffered = IsBuffered();
        _lastInput = 0;
        return wasBuffered;
    }

    public bool IsBuffered() => Time.GetTicksMsec() - _lastInput < _bufferWindow;

    protected override PI_ActionState InputDown(InputEvent @event, out T value)
    {
        _lastInput = Time.GetTicksMsec();
        value = GetInputValue(@event);
        return PI_ActionState.STARTED;
    }

    protected override PI_ActionState InputUp(InputEvent @event, out T value)
    {
        value = GetInputValue(@event);
        return PI_ActionState.STOPPED;
    }
}