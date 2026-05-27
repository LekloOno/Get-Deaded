using Godot;

[GlobalClass]
public partial class UIW_LowLoaded : Control
{
    [Export] private Label? _label;
    // Later use a custom displayable texture rect probably.
    [Export] private Label? _key;
    private KeyBindingSetting? _bind;

    public override void _Ready()
    {
        _bind = KeyBindingSettingsManager.Instance.GetBinding(ACTIONS_Combat.RELOAD);
        
        OnBindChanged(this, 0);
        
        _bind.Changed += OnBindChanged;
    }

    private void OnBindChanged(GodotObject sender, Variant value)
    {
        if (_bind == null)
            return;

        if (_key == null)
            return;

        EditableInputEvent input;
        
        if (!_bind.TryGetBind(0, out input) &&
            !_bind.TryGetBind(1, out input) &&
            !_bind.TryGetBind(2, out input))
            return;

        InputEvent inputEvent = input.InputEvent;

        if (inputEvent is InputEventMouseButton mouseButton)
            _key.Text = "Mouse " + mouseButton.ButtonIndex;
        else if (inputEvent is InputEventKey keyButton)
            _key.Text = OS.GetKeycodeString(
                DisplayServer.KeyboardGetLabelFromPhysical(keyButton.PhysicalKeycode)
            );
    }
}