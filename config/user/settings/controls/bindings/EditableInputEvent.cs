
using Godot;

[GlobalClass]
public partial class EditableInputEvent : GodotObject
{
    public EditableInputEvent(){}

    public EditableInputEvent(StringName action, InputEvent init)
    {
        _action = action;
        InputEvent = init;

        if (init.TryToString(out string eventKey))
            EventKey = eventKey;
        else
            EventKey = string.Empty;

        if (InputEvent != null)
            InputMap.ActionAddEvent(_action, InputEvent);
    }

    private StringName _action;
    public string EventKey {get; private set;}
    public InputEvent InputEvent {get; private set;}

    public bool TryUpdateValue(GodotObject sender, InputEvent value)
    {
        if (InputEvent == value)
            return true;

        if (InputEvent != null)
            InputMap.ActionEraseEvent(_action, InputEvent);

        InputEvent = value;
        if (InputEvent != null)
            InputMap.ActionAddEvent(_action, InputEvent);

        EmitSignal(SignalName.Changed, sender, value);

        return true;
    }

    [Signal] public delegate void ChangedEventHandler(GodotObject sender, InputEvent value);
}