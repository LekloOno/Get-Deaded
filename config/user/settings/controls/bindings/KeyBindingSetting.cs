using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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

        GD.Print(actionName, " created");
    }

    public override string Section => UserSettingsSection.Inputs;
    public override string Key {get;}
    private Array<EditableInputEvent> _keyBinds;
    public override Variant DefaultFallBack() =>
        InputMap.ActionGetEvents(Key);

    public override Variant Serialized(Variant value)
    {
        GD.Print("serializing");

        if (value.Obj is not Array<EditableInputEvent> inputEvents)
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
            GD.Print("prout");
            effectiveValue = Value;
            return false;
        }

        Array<InputEvent> inputEvents = value.AsGodotArray<InputEvent>();
        if (inputEvents == null)
        {
            GD.Print("prout");
            effectiveValue = Value;
            return false;
        }

        int count = Mathf.Min(_keyBinds.Count, inputEvents.Count);
        for (int i = 0; i < count; i++)
            _keyBinds[i].TryUpdateValue(this, inputEvents[i]);
            
        //effectiveValue = _keyBinds;
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
}