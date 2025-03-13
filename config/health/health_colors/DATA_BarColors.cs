using Godot;

[GlobalClass]
public partial class DATA_BarColors : Resource
{
    [Export] public Color Body {get; private set;}
    [Export] public Color Tail {get; private set;}
}
