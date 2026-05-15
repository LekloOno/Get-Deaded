using Godot;
using TraGUS;

public partial class VsyncModeSetting : UserSetting
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "vsync_mode";

    public override Variant DefaultFallBack() =>
        (int)DisplayServer.VSyncMode.Disabled;

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType != Variant.Type.Int)
        {
            effectiveValue = Value;
            return false;
        }

        var mode = (DisplayServer.VSyncMode)(int)value;
        DisplayServer.WindowSetVsyncMode(mode);
        effectiveValue = value;

        return true;
    }
}