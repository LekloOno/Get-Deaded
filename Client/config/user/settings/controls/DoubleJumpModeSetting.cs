using System;
using Godot;
using TraGUS.DotNet.Conversion;

public partial class DoubleJumpModeSetting : UserSettingFlag<DoubleJumpModeSetting, DoubleJumpMode>
{
    public override string Section => UserSettingsSection.Controls;
    public override string Key => "double_jump_mode";
    public static DoubleJumpMode Mode => Tval;
    public override Variant DefaultFallBack() => (int) DoubleJumpMode.HeightJump;

    protected override bool ProcessTypedValue(DoubleJumpMode typedValue, out DoubleJumpMode effectiveTypedValue)
    {
        effectiveTypedValue = typedValue;
        return true;
    }
}

[Flags]
public enum DoubleJumpMode
{
    /// <summary>
    /// Use the jump input to trigger the double jump.
    /// It is triggered when jump is pressed as the player is passed a height from ground threshold.
    /// </summary>
    HeightJump  = 1 << 0,
    /// <summary>
    /// Use the combination of dash and jump input to trigger the double jump.
    /// It is triggered when dash is pressed, if the player pressed jump a few milliseconds before.
    /// </summary>
    DashJump    = 1 << 1,
    /// <summary>
    /// Use a custom input.
    /// It is triggered when this input is pressed.
    /// </summary>
    Defined     = 1 << 2,
}