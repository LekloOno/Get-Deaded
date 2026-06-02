using Godot;

public partial class SsilSetting : VideoQualitySetting
{
    public override string Key => "screen_space_indirect_lighting";

    protected override void UpdateFrom(VideoQuality quality)
    {
        Environment? env = GetViewport()?.World3D?.Environment;

        if (env == null)
        {
            GD.PrintErr("[ScreenSpaceIndirectLighting] no Environment found.");
            return;
        }

        UpdateEnvironment(env);
    }
    
    private static SsilQuality From(VideoQuality quality)
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
            env.SsilEnabled = false;
            return;
        }

        env.SsilEnabled = true;

        SsilQuality settings = From(Quality);
        
        RenderingServer.EnvironmentSetSsilQuality(
            settings.Quality,
            true,
            1f,
            4,
            50f,
            300f);
    }

    private readonly static SsilQuality Minimal = new(
        RenderingServer.EnvironmentSsilQuality.VeryLow);

    private readonly static SsilQuality Low = new(
        RenderingServer.EnvironmentSsilQuality.Low);

    private readonly static SsilQuality Medium = new(
        RenderingServer.EnvironmentSsilQuality.Medium);

    private readonly static SsilQuality High = new(
        RenderingServer.EnvironmentSsilQuality.High);

    private readonly static SsilQuality Ultra = new(
        RenderingServer.EnvironmentSsilQuality.Ultra);
}

public record SsilQuality(
    RenderingServer.EnvironmentSsilQuality Quality);