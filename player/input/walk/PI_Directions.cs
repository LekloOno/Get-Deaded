using Godot;

public class PI_Direction
{
    public static readonly PI_Direction FORWARD = new("move_forward");
    public static readonly PI_Direction BACKWARD = new("move_backward");
    public static readonly PI_Direction LEFT = new("move_left");
    public static readonly PI_Direction RIGHT = new("move_right");

    public string Value { get; }

    private PI_Direction(string value) => Value = value;
    public static implicit operator StringName(PI_Direction direction) => new StringName(direction.Value);
}
