using System;
using Godot;

[GlobalClass]
public partial class PI_Jump : Node
{
    [Export] private ulong _bufferWindow = 50;

    public ulong LastJumped {get; private set;} = 0;
    private ulong _lastJumpInput = 0;
    public bool JumpBuffered {get; private set;}

    public EventHandler OnStartInput;

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event.IsActionPressed("jump"))
        {
            _lastJumpInput = Time.GetTicksMsec();
            OnStartInput?.Invoke(this, EventArgs.Empty);
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