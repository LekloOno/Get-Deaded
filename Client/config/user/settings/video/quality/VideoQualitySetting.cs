using Godot;
using TraGUS.DotNet.Conversion;

public abstract partial class VideoQualitySetting<T> : UserSettingEnum<T, VideoQuality> where T: VideoQualitySetting<T>
{
    public override string Section => UserSettingsSection.Video;

    public override Variant DefaultFallBack() => (int) VideoQuality.Medium;
    public static VideoQuality Quality => Tval;

    protected override bool ProcessTypedValue(VideoQuality typedValue, out VideoQuality effectiveTypedValue)
    {
        UpdateFrom(typedValue, out VideoQuality effectiveQuality);
        effectiveTypedValue = effectiveQuality;
        return true;
    }

    protected abstract void UpdateFrom(VideoQuality quality, out VideoQuality effectiveQuality);
}