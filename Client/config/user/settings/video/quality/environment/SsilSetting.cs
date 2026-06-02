using Godot;

public partial class SsilSetting : VideoQualitySetting
{
    public const string KeyString = "screen_space_indirect_lighting";
    public override string Key => KeyString;

    protected override void UpdateFrom(VideoQuality quality, out VideoQuality effectiveQuality)
    {
        Environment? env = GetViewport()?.World3D?.Environment;

        effectiveQuality = quality;

        if (env == null)
        {
            GD.PrintErr("[SsilSetting] no Environment found.");
            return;
        }

        UpdateEnvironmentFrom(env, effectiveQuality);
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

    private static void UpdateEnvironmentFrom(Environment env, VideoQuality quality)
    {       
        if (quality == VideoQuality.Disabled)
        {
            env.SsilEnabled = false;
            return;
        }

        env.SsilEnabled = true;

        SsilQuality settings = From(quality);
        
        RenderingServer.EnvironmentSetSsilQuality(
            settings.Quality,
            true,
            1f,
            4,
            50f,
            300f);
    }

    public static void UpdateEnvironment(Environment env)
        => UpdateEnvironmentFrom(env, Quality);

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