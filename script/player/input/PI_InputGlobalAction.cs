using Godot;

public abstract partial class PI_InputGlobalAction : Node, PI_InputAction
{
    public void EnableAction() => SetProcessUnhandledInput(true);
    public void DisableAction() => SetProcessUnhandledInput(false);
}