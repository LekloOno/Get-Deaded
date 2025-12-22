using Godot;

namespace Pew;

[GlobalClass]
public partial class PC_FlatDir : Node3D
{
    public override void _Ready()
    {
        Node3D parent = GetParent<Node3D>();

        if (parent != null)
            Rotation = parent.Rotation;
        else
            GD.Print("Parent is not a Node3D or does not exist.");
    }
}