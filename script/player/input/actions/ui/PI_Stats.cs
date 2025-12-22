using Godot;

namespace Pew;

[GlobalClass]
public partial class PI_Stats : PI_PressHandler<EmptyInput>
{
    protected override ACTIONS_Action Action => ACTIONS_Ui.STATS;

    protected override void InputDown(InputEvent @event) =>
        Start?.Invoke(this, EmptyInput.NONE);

    protected override void InputUp(InputEvent @event) =>
        Stop?.Invoke(this, EmptyInput.NONE);

    protected override EmptyInput GetInputValue(InputEvent @event) => EmptyInput.NONE;

    public override void _UnhandledKeyInput(InputEvent @event) =>
        HandleInput(@event);

    public override void EnableAction() =>
        SetProcessUnhandledKeyInput(true);

    public override void DisableAction() =>
        SetProcessUnhandledKeyInput(false);
}