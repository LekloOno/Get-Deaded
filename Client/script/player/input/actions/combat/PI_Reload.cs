using Godot;

[GlobalClass]
public partial class PI_Reload : PI_BufferedHandler<EmptyInput>
{
    protected override ACTIONS_Action Action => ACTIONS_Combat.RELOAD;
    
    public override void _UnhandledInput(InputEvent @event) => HandleInput(@event);

    public override void EnableAction() => SetProcessUnhandledInput(true);

    protected override void DisableBufferAction() => SetProcessUnhandledInput(false);

    protected override EmptyInput GetInputValue(InputEvent @event) => EmptyInput.NONE;
}