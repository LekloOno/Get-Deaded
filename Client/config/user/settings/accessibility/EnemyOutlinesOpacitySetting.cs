using Godot;
using TraGUS;

public partial class EnemyOutlinesOpacitySetting : UserSetting
{
    public override string Section => UserSettingsSection.Accessibility;
    public override string Key => "enemy_outlines_opacity";

    public override Variant DefaultFallBack() => 1f;

    // Explicit static wrapper for c# binding
    public static float Opacity;

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType != Variant.Type.Float &&
            value.VariantType != Variant.Type.Int)
        {
            effectiveValue = Value;
            return false;
        }

        float opacity = (float) value;
        effectiveValue = opacity;
        Opacity = opacity;
        RenderingServer.GlobalShaderParameterSet("enemy_outlines_opacity", opacity);

        return true;
    }
}