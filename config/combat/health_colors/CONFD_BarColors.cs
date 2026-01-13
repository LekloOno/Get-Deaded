using Godot;

[GlobalClass]
public partial class CONFD_BarColors : Resource, CONFD_IBarColors
{
	[Export] public Color Body {get; private set;}
	[Export] public Color Tail {get; private set;}

	public static CONFD_BarColors Default =>
		new() { Body = CONF_HitColors.Colors.Normal, Tail = CONF_HitColors.Colors.Critical };
}
