using Godot;
using TraGUS;

public partial class EnemiesColorSetting : UserSetting
{
    public override string Section => UserSettingsSection.Accessibility;
    public override string Key => "enemies_color";

    public override Variant DefaultFallBack() => new Color(1f, 0f, 72f/255f);

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType != Variant.Type.Color)
        {
            effectiveValue = Value;
            return false;
        }

        effectiveValue = value;
        CONF_HitColors.Colors.Critical = (Color)value;

        return true;
    }
}