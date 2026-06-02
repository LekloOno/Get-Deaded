using Godot;

public partial class DirectionalShadowsSetting : VideoQualitySetting
{
    public const string KeyString = "directional_shadows_quality";
    public override string Key => KeyString;

    protected override void UpdateFrom(VideoQuality quality, out VideoQuality effectiveQuality)
    {
        DirectionalShadowsQuality settings = From(quality);

        RenderingServer.DirectionalSoftShadowFilterSetQuality(settings.FilterQuality);
        RenderingServer.DirectionalShadowAtlasSetSize(settings.Size, true);

        effectiveQuality = quality;

        foreach (Node node in GetTree().GetNodesInGroup("Sun"))
        {
            if (node is DirectionalLight3D sun)
                UpdateLightFrom(sun, effectiveQuality);
        }
    }

    private static void UpdateLightFrom(DirectionalLight3D light, VideoQuality quality)
    {   
        DirectionalShadowsQuality settings = From(quality);

        light.DirectionalShadowMode = settings.ShadowMode;
        light.DirectionalShadowMaxDistance = settings.MaxDistance;
        light.DirectionalShadowSplit1 = settings.Split1;
        light.ShadowBlur = settings.Blur;  
    }

    public static void UpdateLight(DirectionalLight3D light) =>
        UpdateLightFrom(light, Quality);

    private static DirectionalShadowsQuality From(VideoQuality quality)
    {
        return quality switch
        {
            VideoQuality.Disabled => Disabled,
            VideoQuality.Minimal => Minimal,
            VideoQuality.Low => Low,
            VideoQuality.Medium => Medium,
            VideoQuality.High => High,
            VideoQuality.Ultra => Ultra,
            _ => Disabled,
        };
    }

    private static readonly DirectionalShadowsQuality Disabled = new(
        RenderingServer.ShadowQuality.Hard,
        256,
        DirectionalLight3D.ShadowMode.Orthogonal,
        0.1f,
        0.1f,
        1f);

    private static readonly DirectionalShadowsQuality Minimal = new(
        RenderingServer.ShadowQuality.Hard,
        2048,
        DirectionalLight3D.ShadowMode.Orthogonal,
        15f,
        0.1f,
        1f);

    private static readonly DirectionalShadowsQuality Low = new(
        RenderingServer.ShadowQuality.SoftLow,
        2048,
        DirectionalLight3D.ShadowMode.Orthogonal,
        20f,
        0.1f,
        1f);

    private static readonly DirectionalShadowsQuality Medium = new(
        RenderingServer.ShadowQuality.SoftMedium,
        4096,
        DirectionalLight3D.ShadowMode.Parallel2Splits,
        25f,
        0.15f,
        3f);

    private static readonly DirectionalShadowsQuality High = new(
        RenderingServer.ShadowQuality.SoftHigh,
        8192,
        DirectionalLight3D.ShadowMode.Parallel4Splits,
        60f,
        0.1f,
        1f);

    private static readonly DirectionalShadowsQuality Ultra = new(
        RenderingServer.ShadowQuality.SoftUltra,
        8192,
        DirectionalLight3D.ShadowMode.Parallel4Splits,
        120f,
        0.1f,
        1f);
}

public record DirectionalShadowsQuality(
    RenderingServer.ShadowQuality FilterQuality,
    int Size,
    DirectionalLight3D.ShadowMode ShadowMode,
    float MaxDistance,
    float Split1,
    float Blur
) : ShadowsQuality(FilterQuality, Size);