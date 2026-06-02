using Godot;

public partial class AmbientOcclusionSetting : VideoQualitySetting
{
    public override string Key => "ambient_occlusion";

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
        env.SsaoEnabled = Quality != VideoQuality.Disabled;
        
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

    private static AmbientOcclusionQuality Minimal = new(
        RenderingServer.EnvironmentSsaoQuality.VeryLow);

    private static AmbientOcclusionQuality Low = new(
        RenderingServer.EnvironmentSsaoQuality.Low);

    private static AmbientOcclusionQuality Medium = new(
        RenderingServer.EnvironmentSsaoQuality.Medium);

    private static AmbientOcclusionQuality High = new(
        RenderingServer.EnvironmentSsaoQuality.High);

    private static AmbientOcclusionQuality Ultra = new(
        RenderingServer.EnvironmentSsaoQuality.Ultra);
}

public record AmbientOcclusionQuality(
    RenderingServer.EnvironmentSsaoQuality Quality);