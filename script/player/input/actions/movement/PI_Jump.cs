using System;
using Godot;

/// <summary>
/// Handles buffer input for the jump.
/// 
/// The Input values corresponds to the jump strength. We could later use the input strength for analog jump inputs.
/// </summary>
[GlobalClass]
public partial class PI_Jump : PI_BufferedHandler<float>
{
    public ulong LastJumped {get; private set;} = 0;
    protected override ACTIONS_Action Action => ACTIONS_Movement.JUMP;
    public override void _UnhandledKeyInput(InputEvent @event) => HandleInput(@event);
    protected override float GetInputValue(InputEvent @event) => 1f;
    public void SetLastJumped() => LastJumped = PHX_Time.ScaledTicksMsec;

    public override void EnableAction() => SetProcessUnhandledKeyInput(true);
    protected override void DisableBufferAction() => SetProcessUnhandledKeyInput(false);
}