using System;
using Godot;

[GlobalClass]
public partial class PI_Jump : Node
{
    [Export] private ulong _bufferWindow = 50;

    public ulong LastJumped {get; private set;} = 0;
    private ulong _lastJumpInput = 0;
    public bool JumpDown {get; private set;}
    public bool JumpBuffered {get; private set;}

    public EventHandler OnStartInput;
    public EventHandler OnStopInput;

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event.IsActionPressed("jump"))
        {
            JumpDown = true;
            _lastJumpInput = Time.GetTicksMsec();
            OnStartInput?.Invoke(this, EventArgs.Empty);
        } else if (JumpDown && @event.IsActionReleased("jump"))
        {
            JumpDown = false;
            OnStopInput?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public bool UseBuffer()
    {
        bool wasBuffered = IsBuffered();
        _lastJumpInput = 0;
        return wasBuffered;
    }

    public bool IsBuffered() => Time.GetTicksMsec() - _lastJumpInput < _bufferWindow;

    public void SetLastJumped() => LastJumped = Time.GetTicksMsec();
}