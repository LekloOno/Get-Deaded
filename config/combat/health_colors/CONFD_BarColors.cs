using Godot;

namespace Pew;

[GlobalClass]
public partial class CONFD_BarColors : Resource
{
    [Export] public Color Body {get; private set;}
    [Export] public Color Tail {get; private set;}
}
