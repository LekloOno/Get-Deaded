using System;
using Godot;

[GlobalClass]
public partial class PI_Jump : PI_Action<float>
{
    [Export] private ulong _bufferWindow = 50;

    public ulong LastJumped {get; private set;} = 0;
    public bool JumpBuffered {get; private set;}
    private ulong _lastJumpInput = 0;
    protected override ACTIONS_Action Action => ACTIONS_Movement.JUMP;

    public bool UseBuffer()
    {
        bool wasBuffered = IsBuffered();
        _lastJumpInput = 0;
        return wasBuffered;
    }

    public bool IsBuffered() => Time.GetTicksMsec() - _lastJumpInput < _bufferWindow;

    public void SetLastJumped() => LastJumped = Time.GetTicksMsec();

    public override PI_ActionState InputDown(InputEvent @event, out float value)
    {
        _lastJumpInput = Time.GetTicksMsec();
        value = 1f;
        return PI_ActionState.STARTED;
    }

    public override PI_ActionState InputUp(InputEvent @event, out float value)
    {
        value = 1f;
        return PI_ActionState.STOPPED;
    }
}