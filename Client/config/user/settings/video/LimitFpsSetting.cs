using Godot;
using TraGUS;

public partial class LimitFpsSetting : UserSetting
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "limit_fps";

    public override Variant DefaultFallBack() => true;
    public static bool LimitFps { get; private set; }

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType != Variant.Type.Bool)
        {
            effectiveValue = Value;
            return false;
        }

        if ((bool)value)
            Engine.MaxFps = MaxFpsSetting.MaxFps;
        else
            Engine.MaxFps = 0;

        LimitFps = (bool)value;
        effectiveValue = value;
        return true;
    }
}