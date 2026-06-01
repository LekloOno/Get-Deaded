using Godot;

public partial class DirectionalShadowsSetting : VideoQualitySetting
{
    public override string Key => "directional_shadows_quality";

    protected override void UpdateFrom(VideoQuality quality)
    {
        ShadowsQuality settings = From(Quality);

        RenderingServer.DirectionalSoftShadowFilterSetQuality(settings.FilterQuality);
        RenderingServer.DirectionalShadowAtlasSetSize(settings.Size, true);
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
            _ => Low,
        };
    }

    private static readonly ShadowsQuality Minimal = new(RenderingServer.ShadowQuality.Hard, 2048);
    private static readonly ShadowsQuality Low = new(RenderingServer.ShadowQuality.SoftLow, 2048);
    private static readonly ShadowsQuality Medium = new(RenderingServer.ShadowQuality.SoftMedium, 4096);
    private static readonly ShadowsQuality High = new(RenderingServer.ShadowQuality.SoftHigh, 8192);
    private static readonly ShadowsQuality Ultra = new(RenderingServer.ShadowQuality.SoftUltra, 8192);
}