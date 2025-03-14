using System;
using Godot;

[GlobalClass]
public partial class PI_Jump : PI_BufferedHandler
{
    public ulong LastJumped {get; private set;} = 0;
    protected override ACTIONS_Action Action => ACTIONS_Movement.JUMP;
    public override void _UnhandledKeyInput(InputEvent @event) => HandleInput(@event);
    public void SetLastJumped() => LastJumped = Time.GetTicksMsec();
}