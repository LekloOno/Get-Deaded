using Godot;
using TraGUS;

public partial class LimitFpsSetting : UserSetting
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "limit_fps";

    public override Variant DefaultFallBack() => true;

    private UserSetting _maxFps;

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType != Variant.Type.Bool)
        {
            effectiveValue = Value;
            return false;
        }

        if ((bool)value)
            Engine.MaxFps = (int) _maxFps.Value;
        else
            Engine.MaxFps = 0;

        effectiveValue = value;
        return true;
    }

    protected override void PreInitialize()
    {
        if (UserSettingsServer.GetSetting(UserSettingsSection.Video, "max_fps", out UserSetting maxFps))
            _maxFps = maxFps;
    }
}