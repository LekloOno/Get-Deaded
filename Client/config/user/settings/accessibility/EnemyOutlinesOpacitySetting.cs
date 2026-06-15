using Godot;
using TraGUS.DotNet.Conversion.Numeric;

public partial class EnemyOutlinesOpacitySetting : UserSettingFloat<EnemyOutlinesOpacitySetting>
{
    public override string Section => UserSettingsSection.Accessibility;
    public override string Key => "enemy_outlines_opacity";

    public override Variant DefaultFallBack() => 1f;
    public static float Opacity => Tval;

    protected override bool ProcessTypedValue(float typedValue, out float effectiveTypedValue)
    {
        if (typedValue < 0f)
        {
            effectiveTypedValue = 0f;
            return false;
        }

        effectiveTypedValue = typedValue;
        RenderingServer.GlobalShaderParameterSet("enemy_outlines_opacity", typedValue);
        return true;
    }
}