using Godot;
using TraGUS.DotNet.Conversion;

public partial class ResolutionSetting : UserSettingVector2I<ResolutionSetting>
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "resolution";

    public override Variant DefaultFallBack() =>
        new Vector2I(1920, 1080);

    protected override bool ProcessTypedValue(Vector2I typedValue, out Vector2I effectiveTypedValue)
    {
        if (typedValue.X < 200 ||
            typedValue.Y < 150)
        {
            effectiveTypedValue = new(800, 600);
            return false;
        }

        if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Windowed)
            GetWindow().Size = typedValue;
        GetWindow().ContentScaleSize = typedValue;

        effectiveTypedValue = typedValue;
        return true;
    }
}