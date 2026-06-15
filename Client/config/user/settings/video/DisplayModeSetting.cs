using Godot;
using TraGUS;
using TraGUS.DotNet.Conversion;

public partial class DisplayModeSetting : UserSettingEnum<DisplayModeSetting, DisplayServer.WindowMode>
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "display_mode";

    public override Variant DefaultFallBack() =>
        (int)DisplayServer.WindowMode.Fullscreen;

    protected override bool ProcessTypedValue(DisplayServer.WindowMode typedValue, out DisplayServer.WindowMode effectiveTypedValue)
    {
        DisplayServer.WindowSetMode(typedValue);
        effectiveTypedValue = typedValue;
        return true;
    }
}