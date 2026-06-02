using System;

public enum VideoQuality
{
    Disabled,
    Minimal,
    Low,
    Medium,
    High,
    Ultra,
}

[Flags]
public enum VideoQualityMask
{
    Disabled = 1 << 0,
    Minimal = 1 << 1,
    Low     = 1 << 2,
    Medium  = 1 << 3,
    High    = 1 << 4,
    Ultra   = 1 << 5
}

public static class VideoQualitySettingExt
{
    public static VideoQualityMask ToMask(this VideoQuality quality)
    {
        return quality switch
        {
            VideoQuality.Disabled => VideoQualityMask.Disabled,
            VideoQuality.Minimal => VideoQualityMask.Minimal,
            VideoQuality.Low     => VideoQualityMask.Low,
            VideoQuality.Medium  => VideoQualityMask.Medium,
            VideoQuality.High    => VideoQualityMask.High,
            VideoQuality.Ultra   => VideoQualityMask.Ultra,
            _                           => VideoQualityMask.Disabled
        };
    }
}