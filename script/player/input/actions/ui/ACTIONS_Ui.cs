using Godot;

namespace Pew;

public class ACTIONS_Ui(StringName value) : ACTIONS_Action(value)
{
    public static readonly ACTIONS_Movement STATS = new("stats");
}