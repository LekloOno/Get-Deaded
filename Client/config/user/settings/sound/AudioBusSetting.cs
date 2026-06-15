using System;
using Godot;
using TraGUS.DotNet.Conversion.Numeric;

[GlobalClass]
public partial class AudioBusSetting : UserSettingFloat<AudioBusSetting>
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

    private void SetLinearDb(float linearDb)
    {
        float volumeDb = Mathf.LinearToDb(linearDb);
        AudioServer.SetBusVolumeDb(_busIndex, volumeDb);
    }

    protected override bool ProcessTypedValue(float typedValue, out float effectiveTypedValue)
    {
        // Godot init
        if (typedValue == -1)
        {
            effectiveTypedValue = Tval;
            return false;
        }

        if (typedValue > 1f || typedValue < 0f)
        {
            typedValue = Mathf.Clamp(typedValue, 0f, 1f);
            effectiveTypedValue = typedValue;
            SetLinearDb(typedValue);
            return false;
        }

        effectiveTypedValue = typedValue;
        SetLinearDb(typedValue);
        return true;
    }

}