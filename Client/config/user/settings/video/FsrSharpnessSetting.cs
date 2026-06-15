using Godot;
using TraGUS.DotNet.Conversion.Numeric;

public partial class FsrSharpnessSetting : UserSettingFloat<FsrSharpnessSetting>
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "fsr_sharpness";

    public override Variant DefaultFallBack() => 0.5f;

    protected override bool ProcessTypedValue(float typedValue, out float effectiveTypedValue)
    {
        GetTree().Root.FsrSharpness = typedValue;
        effectiveTypedValue = typedValue;
        return true;
    }
}