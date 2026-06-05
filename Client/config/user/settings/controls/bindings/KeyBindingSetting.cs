using System.Collections.Generic;
using Godot;
using Godot.Collections;
using TraGUS;

/// <summary>
/// This setting's inner variant value is a string representative of its _keyBinds list.
/// </summary>
[GlobalClass]
public partial class KeyBindingSetting : UserSetting
{
    public KeyBindingSetting(){}

    public KeyBindingSetting(string actionName)
    {
        Key = actionName;
        _keyBinds = [
            new(actionName, null),
            new(actionName, null),
            new(actionName, null),
        ];

        foreach (EditableInputEvent bind in _keyBinds)
            bind.Changed += Update;
    }

    private void Update(GodotObject sender, InputEvent value)
    {
        UserSettingsServer.Instance.Config.SetValue(
            Section.ToSnakeCase(),
            Key,
            Serialized(_keyBinds)
        );

        UserSettingsServer.Instance.HasBeenModified = true;

        EmitSignal(UserSetting.SignalName.Changed, sender, _keyBinds);
    }

    public override string Section => UserSettingsSection.Inputs;
    public override string Key {get;}
    private Array<EditableInputEvent> _keyBinds;
    private Array<InputEvent> _initBinds;
    public override Variant DefaultFallBack() =>
        _initBinds;

    public override Variant Serialized(Variant value)
    {
        if (value.VariantType != Variant.Type.Array)
            return "[]";

        Array<EditableInputEvent> inputEvents = value.AsGodotArray<EditableInputEvent>();
        if (inputEvents == null)
            return "[]";

        if (inputEvents.TryToStringList(out string serialized))
            return serialized;
            
        return "[]";
    }

    public override bool TryDeserialize(Variant value, out Variant deserialized)
    {
        if (value.VariantType != Variant.Type.String)
        {
            deserialized = new Array<InputEvent>();
            return false;
        }

        string stringVal = (string)value;
        if (!stringVal.TryToInputEventList(out List<InputEvent> inputEvents))
        {
            deserialized = new Array<InputEvent>();
            return false;            
        }

        deserialized = new Array<InputEvent>(inputEvents);
        return true;
    }

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType != Variant.Type.Array)
        {
            effectiveValue = Value;
            return false;
        }

        Array<InputEvent> inputEvents = value.AsGodotArray<InputEvent>();
        if (inputEvents == null)
        {
            effectiveValue = Value;
            return false;
        }

        int count = Mathf.Min(_keyBinds.Count, inputEvents.Count);
        for (int i = 0; i < count; i++)
            _keyBinds[i].TryUpdateValue(this, inputEvents[i]);
            
        effectiveValue = _keyBinds;
        return true;
    }

    public bool TryGetBind(int index, out EditableInputEvent bind)
    {
        if (index > _keyBinds.Count)
        {
            bind = null;
            return false;
        }

        bind = _keyBinds[index];
        return true;
    }

    public EditableInputEvent GdTryGetBind(int index)
    {
        TryGetBind(index, out EditableInputEvent bind);
        return bind;
    }

    protected override void PreInitialize()
    {
        _initBinds = InputMap.ActionGetEvents(Key);
        InputMap.ActionEraseEvents(Key);
    }
}