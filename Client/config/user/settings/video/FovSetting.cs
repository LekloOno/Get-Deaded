using Godot;
using TraGUS;

public partial class FovSetting : UserSetting
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "fov";

    private PC_Settings _playerCameraSettings;

    public override Variant DefaultFallBack() => 115f;

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType != Variant.Type.Int
        && value.VariantType != Variant.Type.Float)
        {
            effectiveValue = Value;
            return false;
        }

        float fov = (float)value;
        if (fov < 60f)
        {
            effectiveValue = 60f;
            _playerCameraSettings.HorizontalFov = 60f;
            return false;
        }

        _playerCameraSettings.HorizontalFov = fov;
        effectiveValue = value;

        return true;
    }

    protected override void PreInitialize()
    {
        _playerCameraSettings = ResourceLoader.Load<PC_Settings>("res://config/player/player_camera_settings.tres");
    }
}