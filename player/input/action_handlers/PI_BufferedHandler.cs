using Godot;

public abstract partial class PI_BufferedHandler : PI_PressHandler<float>
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

    public override PI_ActionState InputDown(InputEvent @event, out float value)
    {
        _lastInput = Time.GetTicksMsec();
        value = 1f;
        return PI_ActionState.STARTED;
    }

    public override PI_ActionState InputUp(InputEvent @event, out float value)
    {
        value = 1f;
        return PI_ActionState.STOPPED;
    }
}