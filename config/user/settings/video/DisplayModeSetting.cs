using Godot;
using TraGUS;

public partial class DisplayModeSetting : UserSetting
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "display_mode";

    public override Variant DefaultFallBack() =>
        (int)DisplayServer.WindowMode.Fullscreen;

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType != Variant.Type.Int)
        {
            effectiveValue = Value;
            return false;
        }

        int id = (int)value;

/*
        if (!Enum.IsDefined(typeof(DisplayServer.WindowMode), id))
        {
            effectiveValue = Value;
            return false;
        }
*/  
        var mode = (DisplayServer.WindowMode)id;
        DisplayServer.WindowSetMode(mode);
        effectiveValue = value;

        return true;
    }
}