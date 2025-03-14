using Godot;

public abstract class PI_Action(StringName value)
{
    public StringName Value { get; } = value;
    public static implicit operator StringName(PI_Action direction) => direction.Value;
}
