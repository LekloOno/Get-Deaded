using System;
using Godot;
using TraGUS;

public partial class PositionalShadowsSetting : UserSetting
{
    public override string Section => UserSettingsSection.Video;
    public override string Key => "positional_shadow_quality";

    public override Variant DefaultFallBack() => (int) VideoQualitySetting.Medium;
    public static VideoQualitySetting Quality { get; private set; } = VideoQualitySetting.Medium;

    protected override bool ProcessValue(Variant value, out Variant effectiveValue)
    {
        if (value.VariantType != Variant.Type.Int)
        {
            effectiveValue = Value;
            return false;
        }

        var intVal = (int) value;

        if (!Enum.IsDefined(typeof(VideoQualitySetting), intVal))
        {
            effectiveValue = Value;
            return false;
        }

        effectiveValue = value;
        Quality = (VideoQualitySetting) intVal;

        ShadowsQualitySettings settings = From(Quality);
        
        RenderingServer.PositionalSoftShadowFilterSetQuality(settings.FilterQuality);
        RenderingServer.ViewportSetPositionalShadowAtlasSize(GetViewport().GetViewportRid(), settings.Size, true);

        return true;
    }

    private static ShadowsQualitySettings From(VideoQualitySetting quality)
    {
        return quality switch
        {
            VideoQualitySetting.Minimal => Minimal,
            VideoQualitySetting.Low => Low,
            VideoQualitySetting.Medium => Medium,
            VideoQualitySetting.High => High,
            VideoQualitySetting.Ultra => Ultra,
            _ => Low,
        };
    }

    private static readonly ShadowsQualitySettings Minimal = new(RenderingServer.ShadowQuality.Hard, 1024);
    private static readonly ShadowsQualitySettings Low = new(RenderingServer.ShadowQuality.SoftLow, 1024);
    private static readonly ShadowsQualitySettings Medium = new(RenderingServer.ShadowQuality.SoftMedium, 2048);
    private static readonly ShadowsQualitySettings High = new(RenderingServer.ShadowQuality.SoftHigh, 4096);
    private static readonly ShadowsQualitySettings Ultra = new(RenderingServer.ShadowQuality.SoftUltra, 4096);
}