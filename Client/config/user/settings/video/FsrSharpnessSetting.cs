using Godot;
using TraGUS;

public partial class FsrSharpnessSetting : UserSetting
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "fsr_sharpness";

    public override Variant DefaultFallBack() => 0.5f;

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType != Variant.Type.Int
        && value.VariantType != Variant.Type.Float)
        {
            effectiveValue = Value;
            return false;
        }

        var scale = (float)value;
        GetTree().Root.FsrSharpness = scale;
        effectiveValue = value;

        return true;
    }
}