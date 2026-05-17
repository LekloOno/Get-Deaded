using Godot;

public class CONFD_DefaultBarColors : CONFD_IBarColors
{
    public readonly static CONFD_DefaultBarColors Default = new();
    public Color Body => CONF_HitColors.Colors.Normal;
    public Color Tail => CONF_HitColors.Colors.Critical;
}