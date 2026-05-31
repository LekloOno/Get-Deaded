using Godot;
using TraGUS;

public partial class RenderScaleScaleSetting : UserSetting
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "render_scale_scale";

    public override Variant DefaultFallBack() => 1f;
    public static float Scale = 1f;

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (RenderScaleModeSetting.Mode == -1 || ( 
            value.VariantType != Variant.Type.Int &&
            value.VariantType != Variant.Type.Float))
        {
            effectiveValue = Value;
            return false;
        }

        var scale = (float)value;
        Scale = scale;
        effectiveValue = value;
        GetTree().Root.Scaling3DScale = scale;

        return true;
    }
}