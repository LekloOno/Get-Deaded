using System;

public enum VideoQualitySetting
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
    public static VideoQualityMask ToMask(this VideoQualitySetting quality)
    {
        return quality switch
        {
            VideoQualitySetting.Minimal => VideoQualityMask.Minimal,
            VideoQualitySetting.Low     => VideoQualityMask.Low,
            VideoQualitySetting.Medium  => VideoQualityMask.Medium,
            VideoQualitySetting.High    => VideoQualityMask.High,
            VideoQualitySetting.Ultra   => VideoQualityMask.Ultra,
            _                           => VideoQualityMask.None
        };
    }
}