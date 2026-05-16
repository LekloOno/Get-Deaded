using Godot;
using TraGUS;

public partial class MaxFpsSetting : UserSetting
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "max_fps";

    public override Variant DefaultFallBack() => 165;

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType != Variant.Type.Int
        && value.VariantType != Variant.Type.Float
        )
        {
            effectiveValue = Value;
            return false;
        }

        int fps = (int)value;

        if (fps < 20)
        {
            effectiveValue = 20;
            Engine.MaxFps = 20;
            return false;
        }
        //GD.Print(Value);

        Engine.MaxFps = fps;
        effectiveValue = value;

        //GD.Print(Engine.MaxFps);
        return true;
    }
}