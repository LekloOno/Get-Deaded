using Godot;
using TraGUS.DotNet.Conversion.Numeric;

public partial class MaxFpsSetting : UserSettingInt<MaxFpsSetting>
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "max_fps";

    public override Variant DefaultFallBack() => 165;
    public static int MaxFps => Tval;

    protected override bool ProcessTypedValue(int typedValue, out int effectiveTypedValue)
    {
        if (typedValue < 20)
        {
            Apply(20);
            effectiveTypedValue = 20;
            return false;
        }

        Apply(typedValue);
        effectiveTypedValue = typedValue;

        return true;
    }

    private void Apply(int maxFps)
    {
        if (LimitFpsSetting.LimitFps)
            Engine.MaxFps = maxFps;
    }
}