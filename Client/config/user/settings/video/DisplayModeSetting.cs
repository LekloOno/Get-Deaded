using System.Threading.Tasks;
using Godot;
using TraGUS.DotNet.Conversion;

public partial class DisplayModeSetting : UserSettingEnum<DisplayModeSetting, DisplayServer.WindowMode>
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "display_mode";

    public override Variant DefaultFallBack() =>
        (int)DisplayServer.WindowMode.Fullscreen;

    protected override bool ProcessTypedValue(DisplayServer.WindowMode typedValue, out DisplayServer.WindowMode effectiveTypedValue)
    {
        DisplayServer.WindowSetFlag(DisplayServer.WindowFlags.Borderless, typedValue != DisplayServer.WindowMode.Windowed);
        DisplayServer.WindowSetMode(typedValue);

        ResolutionSetting.SetResolution(typedValue, ResolutionSetting.Tval);
        
        effectiveTypedValue = typedValue;

        return true;
    }
}