using Godot;

public abstract partial class PM_Action : Node
{
    [Export] protected PI_Walk _walkProcess;
    [Export] protected PS_Grounded _groundState;
}