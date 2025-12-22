using Godot;

namespace Pew;

public abstract partial class PI_InputGlobalAction : Node, PI_InputAction
{
    public virtual void EnableAction() => SetProcessUnhandledInput(true);
    public virtual void DisableAction() => SetProcessUnhandledInput(false);
}