using Godot;
using TraGUS.DotNet.Conversion.Numeric;

public partial class RenderScaleScaleSetting : UserSettingFloat<RenderScaleScaleSetting>
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "render_scale_scale";

    public override Variant DefaultFallBack() => 1f;
    public static float Scale => Tval;

    protected override bool ProcessTypedValue(float typedValue, out float effectiveTypedValue)
    {
        if (RenderScaleModeSetting.Mode != RenderScaleMode.Disabled)
            GetTree().Root.Scaling3DScale = typedValue;

        effectiveTypedValue = typedValue;
        return true;
    }

}