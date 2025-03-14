using System;
using Godot;

[GlobalClass]
public partial class PI_Jump : PI_ActionInput
{
    [Export] private ulong _bufferWindow = 50;

    public ulong LastJumped {get; private set;} = 0;
    public bool JumpBuffered {get; private set;}
    private ulong _lastJumpInput = 0;
    protected override PI_Action Action => ACTIONS_Movement.JUMP;

    public bool UseBuffer()
    {
        bool wasBuffered = IsBuffered();
        _lastJumpInput = 0;
        return wasBuffered;
    }

    public bool IsBuffered() => Time.GetTicksMsec() - _lastJumpInput < _bufferWindow;

    public void SetLastJumped() => LastJumped = Time.GetTicksMsec();

    public override void InputDown(InputEvent @event) => _lastJumpInput = Time.GetTicksMsec();
    public override void InputUp(InputEvent @event) {}
}