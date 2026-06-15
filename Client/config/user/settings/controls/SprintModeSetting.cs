using System.Diagnostics.CodeAnalysis;
using Godot;
using TraGUS.DotNet.Conversion;

public partial class SprintModeSetting : UserSettingFlag<SprintModeSetting, SprintMode>
{
    public override string Section => UserSettingsSection.Controls;
    public override string Key => "sprint_mode";
    public static SprintMode Mode => Tval;
    public override Variant DefaultFallBack() => (int) SprintMode.Auto;

    protected override bool ProcessTypedValue(SprintMode typedValue, out SprintMode effectiveTypedValue)
    {
        effectiveTypedValue = typedValue;
        return true;
    }
}

public enum SprintMode
{
    Auto,
    Hold,
    Toggle,
}