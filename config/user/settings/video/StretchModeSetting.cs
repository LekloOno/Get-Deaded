using System;
using Godot;
using TraGUS;

public partial class StretchModeSetting : UserSetting
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "stretch_mode";

    public override Variant DefaultFallBack() =>
        (int)Window.ContentScaleAspectEnum.Keep;

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType != Variant.Type.Int)
        {
            effectiveValue = Value;
            return false;
        }

        var mode = (Window.ContentScaleAspectEnum)(int)value;
        GetTree().Root.ContentScaleAspect = mode;
        effectiveValue = value;

        return true;
    }
}