using System;
using Godot;

public partial class PI_JumpProcess : Node
{
    [Export] public ulong BufferWindow {get; private set;} = 30;

    public ulong LastJumped {get; private set;} = 0;
    private ulong _lastJumpInput = 0;
    public bool JumpBuffered {get; private set;}

    public override void _Ready()
    {
        SetProcessPriority(-9);
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionPressed("jump"))
        {
            _lastJumpInput = Time.GetTicksMsec();
        }
    }

    public void SetLastJumped()
    {
        LastJumped = Time.GetTicksMsec();
    }

    public bool UseBuffer()
    {
        bool wasBuffered = Time.GetTicksMsec() - _lastJumpInput < BufferWindow;
        _lastJumpInput = 0;
        return wasBuffered;
    }
}