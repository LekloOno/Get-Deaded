using Godot;
using TraGUS;

public partial class RenderScaleModeSetting : UserSetting
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "render_scale_mode";

    public override Variant DefaultFallBack() =>
        (int)Viewport.Scaling3DModeEnum.Bilinear;

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType != Variant.Type.Int)
        {
            effectiveValue = Value;
            return false;
        }

        var mode = (Viewport.Scaling3DModeEnum)(int)value;
        GetTree().Root.Scaling3DMode = mode;
        effectiveValue = value;

        return true;
    }
}