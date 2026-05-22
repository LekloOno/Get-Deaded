using Godot;
using TraGUS;

public partial class EnemiesColorSetting : UserSetting
{
    public override string Section => UserSettingsSection.Accessibility;
    public override string Key => "enemies_color";

    public override Variant DefaultFallBack() => new Color(1f, 0f, 72f/255f);

    // Explicit static wrapper for c# binding
    public static Color Color;

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType != Variant.Type.Color)
        {
            effectiveValue = Value;
            return false;
        }
        effectiveValue = value;
        //GD.Print(value + " " + Value);
        CONF_HitColors.Colors.Critical = (Color)value;
        Color = (Color) value;
        RenderingServer.GlobalShaderParameterSet("enemy_color", value);

        return true;
    }
}