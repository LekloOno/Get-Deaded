using Godot;

public class ACTIONS_Combat(StringName value) : ACTIONS_Action(value)
{
    public static readonly ACTIONS_Combat PRIMARY = new("primary");
    public static readonly ACTIONS_Combat SECONDARY = new("secondary");
    public static readonly ACTIONS_Combat REVIVE = new("revive");
}