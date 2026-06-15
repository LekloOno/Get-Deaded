using Godot;
using TraGUS.DotNet.Conversion;

public partial class EnemyColorSetting : UserSettingColor<EnemyColorSetting>
{
    public override string Section => UserSettingsSection.Accessibility;
    public override string Key => "enemies_color";

    public override Variant DefaultFallBack() => new Color(1f, 0f, 72f/255f);
    public static Color Color => Tval;

    protected override bool ProcessTypedValue(Color typedValue, out Color effectiveTypedValue)
    {
        effectiveTypedValue = typedValue;
        CONF_HitColors.Colors.Critical = typedValue;
        RenderingServer.GlobalShaderParameterSet("enemy_color", typedValue);
        return true;
    }
}