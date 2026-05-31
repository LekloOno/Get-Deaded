using System;
using Godot;
using TraGUS;

public partial class RenderScaleModeSetting : UserSetting
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "render_scale_mode";
    public static long Mode = (long)Viewport.Scaling3DModeEnum.Bilinear;
    public override Variant DefaultFallBack() =>
        (long)Viewport.Scaling3DModeEnum.Bilinear;

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType != Variant.Type.Int)
        {
            effectiveValue = Value;
            return false;
        }

        var intVal = (long) value;

        if (!Enum.IsDefined(typeof(Viewport.Scaling3DModeEnum), intVal) &&
            intVal != -1)
        {
            effectiveValue = Value;
            return false;
        }

        effectiveValue = value;
        Mode = intVal;

        if (intVal == -1)
        {
            GetTree().Root.Scaling3DMode = Viewport.Scaling3DModeEnum.Bilinear;
            GetTree().Root.Scaling3DScale = 1f;
        }
        else
        {
            var mode = (Viewport.Scaling3DModeEnum)intVal;
            GetTree().Root.Scaling3DMode = mode;
            GetTree().Root.Scaling3DScale = RenderScaleScaleSetting.Scale;
        }

        return true;
    }
}