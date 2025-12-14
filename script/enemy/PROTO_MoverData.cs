using Godot;

[GlobalClass]
public partial class PROTO_MoverData : Resource
{
    [Export] public float Speed {get; private set;}
    [Export] public float Acceleration {get; private set;}
    [Export] public float MinStraffe {get; private set;}
    [Export] public float MaxStraffe {get; private set;}
}