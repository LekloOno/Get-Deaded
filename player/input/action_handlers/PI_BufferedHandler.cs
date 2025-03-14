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

    protected override void InputDown(InputEvent @event)
    {
        _lastInput = Time.GetTicksMsec();
        T value = GetInputValue(@event);
        Start?.Invoke(this, value);
    }

    protected override void InputUp(InputEvent @event)
    {
        T value = GetInputValue(@event);
        Stop?.Invoke(this, value);
    }
}