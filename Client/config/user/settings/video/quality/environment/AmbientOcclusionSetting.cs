using Godot;

public partial class AmbientOcclusionSetting : VideoQualitySetting<AmbientOcclusionSetting>
{
    public const string KeyString = "ambient_occlusion";
    public override string Key => KeyString;

    protected override void UpdateFrom(VideoQuality quality, out VideoQuality effectiveQuality)
    {
        Environment? env = GetViewport()?.World3D?.Environment;

        effectiveQuality = quality;

        if (env == null)
        {
            GD.PrintErr("[AmbientOcclusionSetting] no Environment found.");
            return;
        }

        UpdateEnvironmentFrom(env, effectiveQuality);
    }
    
    private static AmbientOcclusionQuality From(VideoQuality quality)
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

    private static void UpdateEnvironmentFrom(Environment env, VideoQuality quality)
    {
        if (quality == VideoQuality.Disabled)
        {
            env.SsaoEnabled = false;
            return;
        }

        env.SsaoEnabled = true;

        AmbientOcclusionQuality settings = From(quality);
        
        RenderingServer.EnvironmentSetSsaoQuality(
            settings.Quality,
            true,
            1f,
            2,
            50f,
            300f);
    }

    public static void UpdateEnvironment(Environment env)
        => UpdateEnvironmentFrom(env, Quality);

    private readonly static AmbientOcclusionQuality Minimal = new(
        RenderingServer.EnvironmentSsaoQuality.VeryLow);

    private readonly static AmbientOcclusionQuality Low = new(
        RenderingServer.EnvironmentSsaoQuality.Low);

    private readonly static AmbientOcclusionQuality Medium = new(
        RenderingServer.EnvironmentSsaoQuality.Medium);

    private readonly static AmbientOcclusionQuality High = new(
        RenderingServer.EnvironmentSsaoQuality.High);

    private readonly static AmbientOcclusionQuality Ultra = new(
        RenderingServer.EnvironmentSsaoQuality.Ultra);
}

public record AmbientOcclusionQuality(
    RenderingServer.EnvironmentSsaoQuality Quality);