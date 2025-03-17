using Godot;

public abstract class ACTIONS_Action(StringName value)
{
    public StringName Value { get; } = value;
    public static implicit operator StringName(ACTIONS_Action action) => action.Value;
}
