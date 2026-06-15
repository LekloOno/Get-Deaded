using Godot;
using TraGUS.DotNet.Conversion.Numeric;

public partial class EnemyOutlinesThicknessSetting : UserSettingFloat<EnemyOutlinesThicknessSetting>
{
    public override string Section => UserSettingsSection.Accessibility;
    public override string Key => "enemy_outlines_thickness";

    public override Variant DefaultFallBack() => 2f;
    public static float Thickness => Tval;

    protected override bool ProcessTypedValue(float typedValue, out float effectiveTypedValue)
    {
        if (typedValue < 0f)
        {
            effectiveTypedValue = 0f;
            return false;
        }

        effectiveTypedValue = typedValue;
        RenderingServer.GlobalShaderParameterSet("enemy_outlines_thickness", typedValue);
        return true;
    }
}