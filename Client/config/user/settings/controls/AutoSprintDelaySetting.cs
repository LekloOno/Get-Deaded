using Godot;
using TraGUS.DotNet.Conversion.Numeric;

public partial class AutoSprintDelaySetting : UserSettingUlong<AutoSprintDelaySetting>
{
    public const ulong MinimumDelay = 300;
    public ulong Minimum => MinimumDelay;

    public override string Section => UserSettingsSection.Controls;
    public override string Key => "auto_sprint_delay";

    public override Variant DefaultFallBack() => (ulong) 800;
    public static ulong Delay => Tval;
    
    protected override bool ProcessTypedValue(ulong typedValue, out ulong effectiveTypedValue)
    {
        if (typedValue < MinimumDelay)
        {
            effectiveTypedValue = MinimumDelay;
            return false;
        }

        effectiveTypedValue = typedValue;
        return true;
    }
}