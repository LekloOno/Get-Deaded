using Godot;
using TraGUS.DotNet.Conversion.Numeric;

public partial class CmPer360Setting : UserSettingFloat<CmPer360Setting>
{
    public override string Section => UserSettingsSection.Controls;
    public override string Key => "cm_per_360";

    private PC_Settings _playerCameraSettings;

    public override Variant DefaultFallBack() => 25f;

    protected override void PreInitialize()
    {
        _playerCameraSettings = ResourceLoader.Load<PC_Settings>("res://config/player/player_camera_settings.tres");
    }

    protected override bool ProcessTypedValue(float typedValue, out float effectiveTypedValue)
    {
        _playerCameraSettings.CmPer360 = typedValue;
        effectiveTypedValue = typedValue;
        return true;
    }

}