using Godot;
using TraGUS.DotNet.Conversion;

public partial class LimitFpsSetting : UserSettingBool<LimitFpsSetting>
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "limit_fps";

    public override Variant DefaultFallBack() => true;
    public static bool LimitFps => Tval;

    protected override bool ProcessTypedValue(bool typedValue, out bool effectiveTypedValue)
    {
        if (typedValue)
            Engine.MaxFps = MaxFpsSetting.MaxFps;
        else
            Engine.MaxFps = 0;

        effectiveTypedValue = typedValue;
        return true;
    }
}