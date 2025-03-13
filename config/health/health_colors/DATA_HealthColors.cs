using Godot;

[GlobalClass]
public partial class DATA_HealthColors : Resource
{   
    [Export] public DATA_BarColors Health;
    [Export] public DATA_BarColors Shield;
    [Export] public DATA_BarColors Barrier;
    [Export] public DATA_BarColors Armor;
    [Export] public DATA_BarColors Default;
}