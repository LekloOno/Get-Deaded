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
    /// <summary>
    /// Overrides the _bufferWindow for an infinite window, and changes the way the input are buffered. <br/>
    /// In permanent mode, the buffer does not depend on a lifetime anymore. <br/>
    /// Instead, the buffer is filled when it was empty, and the input is pressed, and it is emptied on consumption or when the input is pressed as the buffer was already filled. 
    /// </summary>
    [Export] private bool _permanent = false;
    /// <summary>
    /// Defines if 
    /// </summary>
    [Export] private bool _scaledTime = true;
    private ulong _lastInput = 0;
    private bool _inputBuffered = false;

    private ulong GetLocalTicksMsec() => _scaledTime
            ? PHX_Time.ScaledTicksMsec
            : Time.GetTicksMsec();

    public bool UseBuffer()
    {
        bool wasBuffered = IsBuffered();
        Empty();
        return wasBuffered;
    }

    public bool IsBuffered() =>
        _permanent
        ? _inputBuffered
        : GetLocalTicksMsec() - _lastInput < _bufferWindow;

    /// <summary>
    /// Automatically empties the buffer on disabling.
    /// For further disabling operations, see PI_BufferedHandler.DisableBufferAction().
    /// </summary>
    public sealed override void DisableAction()
    {
        Empty();
        DisableBufferAction();
    }

    /// <summary>
    /// For consistency reasons, Buffer Action Handlers should override DisableBufferAction instead of DisableAction when implementing new disabling operations.
    /// </summary>
    protected abstract void DisableBufferAction();
    
    protected override void InputDown(InputEvent @event)
    {
        _lastInput = GetLocalTicksMsec();
        _inputBuffered = !_inputBuffered;
        T value = GetInputValue(@event);
        Start?.Invoke(this, value);
    }

    protected override void InputUp(InputEvent @event)
    {
        T value = GetInputValue(@event);
        Stop?.Invoke(this, value);
    }

    protected void Empty()
    {
        _lastInput = 0;
        _inputBuffered = false;
    }
}