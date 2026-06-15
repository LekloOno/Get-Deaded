using Godot;
using TraGUS.DotNet.Conversion.Numeric;

public partial class DpiSetting : UserSettingUint<DpiSetting>
{
    public override string Section => UserSettingsSection.Controls;
    public override string Key => "dpi";

    private PC_Settings _playerCameraSettings = null!;

    public override Variant DefaultFallBack() => 1600;

    protected override void PreInitialize()
    {
        _playerCameraSettings = ResourceLoader.Load<PC_Settings>("res://config/player/player_camera_settings.tres");
    }

    protected override bool ProcessTypedValue(uint typedValue, out uint effectiveTypedValue)
    {
        if (typedValue < 0f)
        {
            effectiveTypedValue = 400;
            return false;
        }

        _playerCameraSettings.Dpi = typedValue;
        effectiveTypedValue = typedValue;
        return true;
    }
}