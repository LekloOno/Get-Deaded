using Godot;
using TraGUS.DotNet.Conversion;

public partial class VsyncModeSetting : UserSettingEnum<VsyncModeSetting, DisplayServer.VSyncMode>
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "vsync_mode";

    public override Variant DefaultFallBack() =>
        (int)DisplayServer.VSyncMode.Disabled;

    protected override bool ProcessTypedValue(DisplayServer.VSyncMode typedValue, out DisplayServer.VSyncMode effectiveTypedValue)
    {
        DisplayServer.WindowSetVsyncMode(typedValue);
        effectiveTypedValue = typedValue;
        return true;
    }
}