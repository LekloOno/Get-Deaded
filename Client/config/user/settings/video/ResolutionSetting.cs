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
            SetResolution(DisplayModeSetting.Tval, effectiveTypedValue);
            return false;
        }

        SetResolution(DisplayModeSetting.Tval, typedValue);

        effectiveTypedValue = typedValue;
        return true;
    }

    public static void SetResolution(DisplayServer.WindowMode windowMode, Vector2I resolution)
    {
        if (windowMode == DisplayServer.WindowMode.Windowed)
        {
            Instance.GetWindow().Position = new(0, 25);
            Instance.GetWindow().Size = resolution;
            Instance.GetWindow().ContentScaleSize = resolution;
        }
        else
            Instance.GetWindow().ContentScaleSize = resolution;
    }
}