using Godot;

public abstract partial class PI_InputKeyAction : Node, PI_InputAction
{
    public void EnableAction() => SetProcessUnhandledKeyInput(true);
    public void DisableAction() => SetProcessUnhandledKeyInput(false);
}