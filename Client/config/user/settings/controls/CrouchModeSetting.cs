using Godot;
using TraGUS.DotNet.Conversion;

public partial class CrouchModeSetting : UserSettingEnum<CrouchModeSetting, CrouchMode>
{
    public override string Section => UserSettingsSection.Controls;
    public override string Key => "crouch_mode";
    public static CrouchMode Mode => Tval;
    public override Variant DefaultFallBack() => (int) CrouchMode.Hold;

    protected override bool ProcessTypedValue(CrouchMode typedValue, out CrouchMode effectiveTypedValue)
    {
        effectiveTypedValue = typedValue;
        return true;
    }
}

public enum CrouchMode
{
    Hold,
    Toggle,
}
