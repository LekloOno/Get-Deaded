using Godot;
using TraGUS.DotNet.Conversion.Numeric;

public partial class FovSetting : UserSettingFloat<FovSetting>
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "fov";

    private PC_Settings _playerCameraSettings = null!;

    public override Variant DefaultFallBack() => 115f;

    protected override bool ProcessTypedValue(float typedValue, out float effectiveTypedValue)
    {
        if (typedValue < 60f)
        {
            effectiveTypedValue = 60f;
            _playerCameraSettings.HorizontalFov = 60f;
            return false;
        }

        _playerCameraSettings.HorizontalFov = typedValue;
        effectiveTypedValue = typedValue;

        return true;
    }

    protected override void PreInitialize()
    {
        _playerCameraSettings = ResourceLoader.Load<PC_Settings>("res://config/player/player_camera_settings.tres");
    }
}