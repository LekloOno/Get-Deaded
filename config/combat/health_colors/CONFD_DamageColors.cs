using Godot;

namespace Pew;

[GlobalClass]
public partial class CONFD_DamageColors : Resource
{   
    [Export] public Color Health;
    [Export] public Color Shield;
    [Export] public Color Barrier;
    [Export] public Color Armor;
    [Export] public Color Default;
}