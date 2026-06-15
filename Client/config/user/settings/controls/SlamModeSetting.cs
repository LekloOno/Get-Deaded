using System;
using Godot;
using TraGUS.DotNet.Conversion;

public partial class SlamModeSetting : UserSettingFlag<SlamModeSetting, SlamMode>
{
    public override string Section => UserSettingsSection.Controls;
    public override string Key => "slam_mode";
    public static SlamMode Mode => Tval;
    public override Variant DefaultFallBack() => (int) SlamMode.QuickCrouch;

    protected override bool ProcessTypedValue(SlamMode typedValue, out SlamMode effectiveTypedValue)
    {
        effectiveTypedValue = typedValue;
        return true;
    }

}

[Flags]
public enum SlamMode
{
    /// <summary>
    /// Use the crouch input to trigger the slam.
    /// It is triggered when quick crouch is started and stopped right away.
    /// So, that is a quick tap of crouch in hold mode, and a double tap in toggle mode.
    /// </summary>
    QuickCrouch  = 1 << 0,
    /// <summary>
    /// Use the combination of dash and crouch input to trigger the double jump.
    /// It is triggered when dash is pressed, if the player pressed crouch a few milliseconds before.
    /// </summary>
    DashCrouch    = 1 << 1,
    /// <summary>
    /// Use a custom input.
    /// It is triggered when this input is pressed.
    /// </summary>
    Defined     = 1 << 2,
}