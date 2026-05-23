using Godot;
using TraGUS;

[GlobalClass]
public partial class AudioBusSetting : UserSetting
{
    public AudioBusSetting(){}

    public AudioBusSetting(string busName, int busIndex)
    {
        Key = busName.ToSnakeCase() + "_volume_db";
        _busIndex = busIndex;
    }

    public override string Section => UserSettingsSection.Sound;
    public override string Key {get;}
    
    // Stick to default bus values to fall back, -1 is simply a marker to notify
    // we should let godot initialize bus volume. 
    public override Variant DefaultFallBack() => -1;
    public float LinearDb => (float) Value;
    private int _busIndex;

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType != Variant.Type.Int
        && value.VariantType != Variant.Type.Float
        || (float) value == -1)
        {
            effectiveValue = Value;
            return false;
        }

        float linearDb = (float)value;

        if (linearDb > 1f || linearDb < 0f)
        {
            linearDb = Mathf.Clamp(linearDb, 0f, 1f);
            effectiveValue = linearDb;
            SetLinearDb(linearDb);
            return false;
        }

        effectiveValue = linearDb;
        SetLinearDb(linearDb);
        return true;
    }

    private void SetLinearDb(float linearDb)
    {
        float volumeDb = Mathf.LinearToDb(linearDb);
        AudioServer.SetBusVolumeDb(_busIndex, volumeDb);
    }
}