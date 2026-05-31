using Godot;
using TraGUS;

public partial class MaxFpsSetting : UserSetting
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "max_fps";

    public override Variant DefaultFallBack() => 165;
    public static int MaxFps { get; private set; }

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        int fps;
        if (value.VariantType == Variant.Type.Int)
            fps = (int)value;
        else if (value.VariantType == Variant.Type.Float)
            fps = Mathf.RoundToInt((float)value);
        else
        {
            effectiveValue = Value;
            return false;
        }

        if (fps < 20)
        {
            Apply(20);
            effectiveValue = 20;
            return false;
        }

        Apply(fps);
        effectiveValue = fps;

        return true;
    }

    private void Apply(int maxFps)
    {
        MaxFps = maxFps;
        if (LimitFpsSetting.LimitFps)
            Engine.MaxFps = maxFps;
    }
}