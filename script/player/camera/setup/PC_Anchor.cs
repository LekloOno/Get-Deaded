using Godot;

namespace Pew;

[GlobalClass]
public partial class PC_Anchor : Node3D
{
    [Export] private Node3D _target;

    public override void _Process(double delta)
    {
        GlobalPosition = _target.GetGlobalTransformInterpolated().Origin;
    }
}