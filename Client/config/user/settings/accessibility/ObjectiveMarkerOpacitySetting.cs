using Godot;
using TraGUS.DotNet.Conversion.Numeric;

public partial class ObjectiveMarkerOpacitySetting : UserSettingFloat<ObjectiveMarkerOpacitySetting>
{
    public override string Section => UserSettingsSection.Accessibility;
    public override string Key => "objective_marker_opacity";

    public override Variant DefaultFallBack() => 0.9f;
    public static float Opacity => Tval;

    protected override bool ProcessTypedValue(float typedValue, out float effectiveTypedValue)
    {
        if (typedValue < 0f)
        {
            effectiveTypedValue = 0f;
            return false;
        }

        effectiveTypedValue = typedValue;
        return true;
    }
}