using System;

public enum VideoQuality
{
    Minimal,
    Low,
    Medium,
    High,
    Ultra,
}

[Flags]
public enum VideoQualityMask
{
    None = 0,
    Minimal = 1 << 0,
    Low     = 1 << 1,
    Medium  = 1 << 2,
    High    = 1 << 3,
    Ultra   = 1 << 4
}

public static class VideoQualitySettingExt
{
    public static VideoQualityMask ToMask(this VideoQuality quality)
    {
        return quality switch
        {
            VideoQuality.Minimal => VideoQualityMask.Minimal,
            VideoQuality.Low     => VideoQualityMask.Low,
            VideoQuality.Medium  => VideoQualityMask.Medium,
            VideoQuality.High    => VideoQualityMask.High,
            VideoQuality.Ultra   => VideoQualityMask.Ultra,
            _                           => VideoQualityMask.None
        };
    }
}