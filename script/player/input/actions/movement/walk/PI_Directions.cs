using Godot;

public class PI_Direction(StringName value) : ACTIONS_Action(value)
{
    public static readonly PI_Direction FORWARD = new("move_forward");
    public static readonly PI_Direction BACKWARD = new("move_backward");
    public static readonly PI_Direction LEFT = new("move_left");
    public static readonly PI_Direction RIGHT = new("move_right");
}
