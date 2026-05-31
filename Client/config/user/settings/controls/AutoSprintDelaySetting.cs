
using Godot;
using TraGUS;

public partial class AutoSprintDelaySetting : UserSetting
{
    public override string Section => UserSettingsSection.Controls;
    public override string Key => "auto_sprint_delay";
    public static ulong Delay {get; private set;} = 800;
    public override Variant DefaultFallBack() => (ulong) 800;
    public const ulong MinimumDelay = 300;
    public ulong Minimum => MinimumDelay;

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType is not Variant.Type.Int &&
            value.VariantType is not Variant.Type.Float)
        {
            effectiveValue = Value;
            return false;
        }

        ulong ulongVal = (ulong) value;

        if (ulongVal < MinimumDelay)
        {
            Delay = MinimumDelay;
            effectiveValue = MinimumDelay;
            return false;
        }

        Delay = ulongVal;
        effectiveValue = ulongVal;
        return true;
    }
}