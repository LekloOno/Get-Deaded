using Godot;

public class ACTIONS_Combat(StringName value) : ACTIONS_Action(value)
{
    public static readonly ACTIONS_Combat PRIMARY = new("primary");
    public static readonly ACTIONS_Combat SECONDARY = new("secondary");
    public static readonly ACTIONS_Combat REVIVE = new("revive");
    public static readonly ACTIONS_Combat SWITCH = new("switch");
    public static readonly ACTIONS_Combat HOLSTER = new("holster");
    public static readonly ACTIONS_Combat RELOAD = new("reload");
    public static readonly ACTIONS_Combat MELEE = new("melee");
    public static readonly ACTIONS_Combat SwitchTo0 = new("switch_to_0");
    public static readonly ACTIONS_Combat SwitchTo1 = new("switch_to_1");
    public static readonly ACTIONS_Combat SwitchToMelee = new("switch_to_melee");
}