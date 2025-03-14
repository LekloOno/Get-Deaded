using System;
using Godot;

[GlobalClass]
public partial class PI_Sprint : PI_HoldableHandler<float>
{

    [ExportCategory("Setup")]
    [Export] private PM_Jump _jump;
    [Export] private PI_Walk _walkInput;
    [Export] private PI_CrouchDispatcher _crouchDispatcher;

    public EventHandler OnStartSprinting;
    public EventHandler OnStopSprinting;

    protected override ACTIONS_Action Action => ACTIONS_Movement.SPRINT;
    protected override float GetInputValue(InputEvent @event) => 1f;

    public override void _Ready()
    {
        _jump.OnStart += (o, f) => HandleExternal(PI_ActionState.STOPPED, 1f);
        _walkInput.OnStopOrBackward += (o, f) => HandleExternal(PI_ActionState.STOPPED, 1f);
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (!_crouchDispatcher.IsCrouched)
            HandleInput(@event);
    }

    public override void EnableAction() => SetProcessUnhandledKeyInput(true);
    public override void DisableAction() => SetProcessUnhandledKeyInput(false);
}