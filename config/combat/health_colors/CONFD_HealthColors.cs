using Godot;

[GlobalClass]
public partial class CONFD_HealthColors : Resource
{   
    [Export] public CONFD_BarColors Health;
    [Export] public CONFD_BarColors Shield;
    [Export] public CONFD_BarColors Barrier;
    [Export] public CONFD_BarColors Armor;
    [Export] public CONFD_BarColors Default;
}