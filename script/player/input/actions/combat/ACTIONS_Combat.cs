using Godot;

public class ACTIONS_Combat(StringName value) : ACTIONS_Action(value)
{
    public static readonly ACTIONS_Combat PRIMARY = new("primary");
    public static readonly ACTIONS_Combat SECONDARY = new("secondary");
    public static readonly ACTIONS_Combat REVIVE = new("revive");
    public static readonly ACTIONS_Combat SWITCH = new("switch");
    public static readonly ACTIONS_Combat HOLSTER = new("holster");
    public static readonly ACTIONS_Combat RELOAD = new("reload");
}