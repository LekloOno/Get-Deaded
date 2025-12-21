using Godot;

/// <summary>
/// A simple Buffered Press input implementation.
/// Whenever the input is pressed down, it is stored in buffer.
/// The UseBuffer() method allows to check if any input is stored, and consume it if so.
/// </summary>
/// <typeparam name="T">The input value type.</typeparam>
public abstract partial class PI_BufferedHandler<T> : PI_PressHandler<T>
{
    [Export] private ulong _bufferWindow;
    [Export] private bool _scaledTime = true;
    protected ulong _lastInput = 0;

    private ulong GetLocalTicksMsec() => _scaledTime
            ? PHX_Time.ScaledTicksMsec
            : Time.GetTicksMsec();

    public bool UseBuffer()
    {
        bool wasBuffered = IsBuffered();
        _lastInput = 0;
        return wasBuffered;
    }

    public bool IsBuffered() => GetLocalTicksMsec() - _lastInput < _bufferWindow;

    /// <summary>
    /// Automatically empties the buffer on disabling.
    /// For further disabling operations, see PI_BufferedHandler.DisableBufferAction().
    /// </summary>
    public sealed override void DisableAction()
    {
        _lastInput = 0;
        DisableBufferAction();
    }

    /// <summary>
    /// For consistency reasons, Buffer Action Handlers should override DisableBufferAction instead of DisableAction when implementing new disabling operations.
    /// </summary>
    protected abstract void DisableBufferAction();
    
    protected override void InputDown(InputEvent @event)
    {
        _lastInput = GetLocalTicksMsec();
        T value = GetInputValue(@event);
        Start?.Invoke(this, value);
    }

    protected override void InputUp(InputEvent @event)
    {
        T value = GetInputValue(@event);
        Stop?.Invoke(this, value);
    }

}