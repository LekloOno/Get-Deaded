using Godot;

public partial class PositionalShadowsSetting : VideoQualitySetting
{
    public const string KeyString = "positional_shadows_quality";
    public override string Key => KeyString;

    protected override void UpdateFrom(VideoQuality quality, out VideoQuality effectiveQuality)
    {
        if (quality == VideoQuality.Disabled)
            effectiveQuality = VideoQuality.Minimal;
        else
            effectiveQuality = quality;

        ShadowsQuality settings = From(effectiveQuality);

        RenderingServer.PositionalSoftShadowFilterSetQuality(settings.FilterQuality);
        RenderingServer.ViewportSetPositionalShadowAtlasSize(GetViewport().GetViewportRid(), settings.Size, true);
    }

    private ShadowsQuality From(VideoQuality quality)
    {
        return quality switch
        {
            VideoQuality.Minimal => Minimal,
            VideoQuality.Low => Low,
            VideoQuality.Medium => Medium,
            VideoQuality.High => High,
            VideoQuality.Ultra => Ultra,
            _ => Minimal,
        };
    }

    private static readonly ShadowsQuality Minimal = new(RenderingServer.ShadowQuality.Hard, 1024);
    private static readonly ShadowsQuality Low = new(RenderingServer.ShadowQuality.SoftLow, 1024);
    private static readonly ShadowsQuality Medium = new(RenderingServer.ShadowQuality.SoftMedium, 2048);
    private static readonly ShadowsQuality High = new(RenderingServer.ShadowQuality.SoftHigh, 4096);
    private static readonly ShadowsQuality Ultra = new(RenderingServer.ShadowQuality.SoftUltra, 4096);
}