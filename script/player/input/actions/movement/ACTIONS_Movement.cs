using Godot;

namespace Pew;

public class ACTIONS_Movement(StringName value) : ACTIONS_Action(value)
{
    public static readonly ACTIONS_Movement JUMP = new("jump");
    public static readonly ACTIONS_Movement SPRINT = new("sprint");
}