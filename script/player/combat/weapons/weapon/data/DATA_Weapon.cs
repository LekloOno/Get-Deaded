using Godot;

[GlobalClass]
public partial class DATA_Weapon : Resource
{
    [Export] public string Id {get; private set;}
    [Export] public string DisplayName {get; private set;}
    [Export] public Texture2D Icon {get; private set;}
    [Export] public Color IconColor {get; private set;}
}