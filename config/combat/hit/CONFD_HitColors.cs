using Godot;

[GlobalClass]
public partial class CONFD_HitColors : Resource
{
    [Export] public Color Critical {get; private set;}
    [Export] public Color Normal {get; private set;}
}