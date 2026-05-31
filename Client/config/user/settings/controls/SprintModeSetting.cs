using System;
using Godot;
using TraGUS;

public partial class SprintModeSetting : UserSetting
{
    public override string Section => UserSettingsSection.Controls;
    public override string Key => "sprint_mode";
    public static SprintMode Mode {get; private set;} = SprintMode.Auto;
    public override Variant DefaultFallBack() => (int) SprintMode.Auto;

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType is not Variant.Type.Int)
        {
            effectiveValue = Value;
            return false;
        }

        int intVal = (int) value;
        if (!Enum.IsDefined(typeof(SprintMode), intVal))
        {
            effectiveValue = Value;
            return false;
        }

        Mode = (SprintMode) intVal;
        effectiveValue = intVal;
        return true;
    }
}

public enum SprintMode
{
    Auto,
    Hold,
    Toggle,
}