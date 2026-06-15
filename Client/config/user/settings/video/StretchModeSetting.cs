using Godot;
using TraGUS.DotNet.Conversion;

public partial class StretchModeSetting : UserSettingEnum<StretchModeSetting, Window.ContentScaleAspectEnum>
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "stretch_mode";

    public override Variant DefaultFallBack() =>
        (int)Window.ContentScaleAspectEnum.Keep;

    protected override bool ProcessTypedValue(Window.ContentScaleAspectEnum typedValue, out Window.ContentScaleAspectEnum effectiveTypedValue)
    {
        GetTree().Root.ContentScaleAspect = typedValue;
        effectiveTypedValue = typedValue;
        return true;
    }
}