using Godot;
using TraGUS;

public partial class ResolutionSetting : UserSetting
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "resolution";

    public override Variant DefaultFallBack() =>
        new Vector2I(1920, 1080);

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType != Variant.Type.Vector2I)
        {
            effectiveValue = Value;
            return false;
        }

        Vector2I res = (Vector2I)value;
        effectiveValue = value;
        
        if (DisplayServer.WindowGetMode() == DisplayServer.WindowMode.Windowed)
            GetWindow().Size = res;
        GetWindow().ContentScaleSize = res;

        return true;
    }
}