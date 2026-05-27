using System.Collections.Generic;
using Godot;

public partial class KeyBindingSettingsManager: Node
{
    public static KeyBindingSettingsManager Instance {get; private set;}
    private static Dictionary<StringName, KeyBindingSetting> _keyBindings = [];

    public override void _EnterTree()
    {
        Instance = this;

        foreach (StringName action in InputMap.GetActions())
        {
            if (action.ToString().StartsWith("ui_"))
                continue;

            AddBinding(action);
        }
    }

    public KeyBindingSetting GetBinding(StringName action)
    {
        _keyBindings.TryGetValue(action, out KeyBindingSetting setting);
        return setting;
    }

    private void AddBinding(StringName action)
    {
        KeyBindingSetting setting = new(action);
        if (_keyBindings.TryAdd(action, setting))
            AddChild(setting);
        else
            GD.PushWarning("Could not add bind setting for action name `" + action + "`.");
    }
}