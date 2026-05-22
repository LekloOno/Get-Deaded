using Godot;
using TraGUS;

public partial class EnemyOutlinesThicknessSetting : UserSetting
{
    public override string Section => UserSettingsSection.Accessibility;
    public override string Key => "enemy_outlines_thickness";

    public override Variant DefaultFallBack() => 2f;

    // Explicit static wrapper for c# binding
    public static float Thickness;

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType != Variant.Type.Float &&
            value.VariantType != Variant.Type.Int)
        {
            effectiveValue = Value;
            return false;
        }

        float thickness = (float) value;
        effectiveValue = thickness;
        Thickness = thickness;
        RenderingServer.GlobalShaderParameterSet("enemy_outlines_thickness", thickness);

        return true;
    }
}