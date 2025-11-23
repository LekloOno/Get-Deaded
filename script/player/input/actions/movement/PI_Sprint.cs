using System;
using Godot;

[GlobalClass]
public partial class PI_Sprint : PI_HoldableHandler<EmptyInput>
{

    [ExportCategory("Setup")]
    [Export] private PM_Jump _jump;
    [Export] private PI_Walk _walkInput;
    [Export] private PI_CrouchDispatcher _crouchDispatcher;

    public EventHandler OnStartSprinting;
    public EventHandler OnStopSprinting;

    protected override ACTIONS_Action Action => ACTIONS_Movement.SPRINT;
    protected override EmptyInput GetInputValue(InputEvent @event) => EmptyInput.NONE;

    protected override void HandlerReady()
    {
        _jump.OnStart += (o, f) => HandleExternal(PI_ActionState.STOPPED, new());
        _walkInput.OnStopOrBackward += (o, f) => HandleExternal(PI_ActionState.STOPPED, new());
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (!_crouchDispatcher.IsCrouched)
            HandleInput(@event);
    }

    public override void EnableAction() => SetProcessUnhandledKeyInput(true);
    public override void DisableAction()
    {
        SetProcessUnhandledKeyInput(false);
        HandleExternal(PI_ActionState.STOPPED, new());
    }
}