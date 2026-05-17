using Godot;
using TraGUS;

public partial class DpiSetting : UserSetting
{
    public override string Section => UserSettingsSection.Controls;
    public override string Key => "dpi";

    private PC_Settings _playerCameraSettings;

    public override Variant DefaultFallBack() => 1600;

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType != Variant.Type.Int
        && value.VariantType != Variant.Type.Float)
        {
            effectiveValue = Value;
            return false;
        }

        uint dpi = (uint)value;
        _playerCameraSettings.Dpi = dpi;
        effectiveValue = value;

        return true;
    }

    protected override void PreInitialize()
    {
        _playerCameraSettings = ResourceLoader.Load<PC_Settings>("res://config/player/player_camera_settings.tres");
    }
}