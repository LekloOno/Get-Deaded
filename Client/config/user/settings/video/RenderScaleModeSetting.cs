using Godot;
using TraGUS.DotNet.Conversion;

public partial class RenderScaleModeSetting : UserSettingEnum<RenderScaleModeSetting, RenderScaleMode>
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "render_scale_mode";
    
    public override Variant DefaultFallBack() =>
        (long)Viewport.Scaling3DModeEnum.Bilinear;
    public static RenderScaleMode Mode => Tval;

    protected override bool ProcessTypedValue(RenderScaleMode typedValue, out RenderScaleMode effectiveTypedValue)
    {
        if (typedValue == RenderScaleMode.Disabled)
        {
            GetTree().Root.Scaling3DMode = Viewport.Scaling3DModeEnum.Bilinear;
            GetTree().Root.Scaling3DScale = 1f;
        }
        else
        {
            var mode = (Viewport.Scaling3DModeEnum)(int)typedValue;
            GetTree().Root.Scaling3DMode = mode;
            GetTree().Root.Scaling3DScale = RenderScaleScaleSetting.Scale;
        }

        effectiveTypedValue = typedValue;
        return true;
    }
}

public enum RenderScaleMode
{
    Disabled = -1,
    Bilinear = (int) Viewport.Scaling3DModeEnum.Bilinear,
    Fsr = (int) Viewport.Scaling3DModeEnum.Fsr,
    Fsr2 = (int) Viewport.Scaling3DModeEnum.Fsr2,
}