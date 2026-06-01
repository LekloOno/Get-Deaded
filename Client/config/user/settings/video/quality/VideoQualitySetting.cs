using System;
using Godot;
using TraGUS;

public abstract partial class VideoQualitySetting : UserSetting
{
    public override string Section => UserSettingsSection.Video;

    public override Variant DefaultFallBack() => (int) VideoQuality.Medium;
    public static VideoQuality Quality { get; protected set; } = VideoQuality.Medium;

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType != Variant.Type.Int)
        {
            effectiveValue = Value;
            return false;
        }

        var intVal = (int) value;

        if (!Enum.IsDefined(typeof(VideoQuality), intVal))
        {
            effectiveValue = Value;
            return false;
        }

        effectiveValue = value;
        Quality = (VideoQuality) intVal;

        UpdateFrom(Quality);

        return true;
    }

    protected abstract void UpdateFrom(VideoQuality quality);
}