using Godot;

public abstract partial class PM_Action : Node
{
    [Export] public PI_Walk WalkProcess { get; private set; }
    [Export] public PS_Grounded GroundState { get; private set; }
}