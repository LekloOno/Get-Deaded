using Godot;

public partial class AmbientOcclusionSetting : VideoQualitySetting
{
    public const string KeyString = "ambient_occlusion";
    public override string Key => KeyString;

    protected override void UpdateFrom(VideoQuality quality)
    {
        Environment? env = GetViewport()?.World3D?.Environment;

        if (env == null)
        {
            GD.PrintErr("[AmbientOcclusionSetting] no Environment found.");
            return;
        }

        UpdateEnvironment(env);
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

    public static void UpdateEnvironment(Environment env)
    {
        if (Quality == VideoQuality.Disabled)
        {
            env.SsaoEnabled = false;
            return;
        }

        env.SsaoEnabled = true;

        AmbientOcclusionQuality settings = From(Quality);
        
        RenderingServer.EnvironmentSetSsaoQuality(
            settings.Quality,
            true,
            1f,
            2,
            50f,
            300f);
    }

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