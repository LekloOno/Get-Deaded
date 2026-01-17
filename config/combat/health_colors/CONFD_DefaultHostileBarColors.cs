using Godot;

public class CONFD_DefaultHostileBarColors : CONFD_IBarColors
{
    public readonly static CONFD_DefaultHostileBarColors Default = new();
    public Color Body => CONF_HitColors.Colors.Critical;
    public Color Tail => CONF_HitColors.Colors.Normal;
}