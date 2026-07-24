using Godot;
using TraGUS.DotNet.Conversion;

public partial class UnactiveObjectiveMarkerColorSetting : UserSettingColor<UnactiveObjectiveMarkerColorSetting>
{
    public override string Section => UserSettingsSection.Accessibility;
    public override string Key => "unactive_objective_marker_color";

    public override Variant DefaultFallBack() => new Color(0.5f, 0.5f, 0.5f, 0.9f);
    public static Color Color => Tval;

    protected override bool ProcessTypedValue(Color typedValue, out Color effectiveTypedValue)
    {
        effectiveTypedValue = typedValue;
        return true;
    }
}