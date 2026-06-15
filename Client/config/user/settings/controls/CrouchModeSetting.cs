using System;
using Godot;
using TraGUS;

public partial class CrouchModeSetting : UserSetting
{
    public override string Section => UserSettingsSection.Controls;
    public override string Key => "crouch_mode";
    public static CrouchMode Mode {get; private set;} = CrouchMode.Hold;
    public override Variant DefaultFallBack() => (int) CrouchMode.Hold;

    public static event Action<CrouchMode>? ModeChanged;

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType is not Variant.Type.Int)
        {
            effectiveValue = Value;
            return false;
        }

        int intVal = (int) value;
        if (!Enum.IsDefined(typeof(CrouchMode), intVal))
        {
            effectiveValue = Value;
            return false;
        }

        Mode = (CrouchMode) intVal;
        effectiveValue = intVal;

        ModeChanged?.Invoke(Mode);

        return true;
    }
}

public enum CrouchMode
{
    Hold,
    Toggle,
}
