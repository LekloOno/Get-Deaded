using Godot;

[GlobalClass]
public partial class PI_Reload : PI_BufferedHandler<EmptyInput>
{
    protected override ACTIONS_Action Action => ACTIONS_Combat.RELOAD;
    
    public override void _UnhandledKeyInput(InputEvent @event) => HandleInput(@event);

    public override void EnableAction() => SetProcessUnhandledKeyInput(true);

    protected override void DisableBufferAction() => SetProcessUnhandledKeyInput(false);

    protected override EmptyInput GetInputValue(InputEvent @event) => EmptyInput.NONE;
}