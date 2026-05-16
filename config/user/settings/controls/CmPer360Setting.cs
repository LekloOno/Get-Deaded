using Godot;
using TraGUS;

public partial class CmPer360Setting : UserSetting
{
    public override string Section => UserSettingsSection.Controls;
    public override string Key => "cm_per_360";

    private PC_Settings _playerCameraSettings;

    public override Variant DefaultFallBack() => 25f;

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType != Variant.Type.Int
        && value.VariantType != Variant.Type.Float)
        {
            effectiveValue = Value;
            return false;
        }


        float cmPer360 = (float)value;
        _playerCameraSettings.CmPer360 = cmPer360;
        effectiveValue = value;

        return true;
    }

    protected override void PreInitialize()
    {
        _playerCameraSettings = ResourceLoader.Load<PC_Settings>("res://config/player/player_camera_settings.tres");
    }
}