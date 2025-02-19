using System;
using Godot;

[GlobalClass]
public partial class PI_Jump : Node
{
    [Export] public ulong BufferWindow {get; private set;} = 30;

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
        bool wasBuffered = Time.GetTicksMsec() - _lastJumpInput < BufferWindow;
        _lastJumpInput = 0;
        return wasBuffered;
    }

    public void SetLastJumped() => LastJumped = Time.GetTicksMsec();
}