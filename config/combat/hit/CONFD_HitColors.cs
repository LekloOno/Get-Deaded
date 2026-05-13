using Godot;

[GlobalClass]
public partial class CONFD_HitColors : Resource
{
    [Export] public Color Critical {get; set;}
    [Export] public Color Normal {get; set;}
}