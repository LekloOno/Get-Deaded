using Godot;

public partial class SsrSetting : VideoQualitySetting
{
    public const string KeyString = "screen_space_reflection";
    public override string Key => KeyString;

    protected override void UpdateFrom(VideoQuality quality, out VideoQuality effectiveQuality)
    {
        Environment? env = GetViewport()?.World3D?.Environment;

        if (quality == VideoQuality.Minimal)
            effectiveQuality = VideoQuality.Disabled;
        else
            effectiveQuality = quality;

        if (env == null)
        {
            GD.PrintErr("[SsrSetting] no Environment found.");
            return;
        }

        UpdateEnvironmentFrom(env, effectiveQuality);
    }
    
    private static SsrQuality From(VideoQuality quality)
    {
        return quality switch
        {
            VideoQuality.Low => Low,
            VideoQuality.Medium => Medium,
            VideoQuality.High => High,
            VideoQuality.Ultra => Ultra,
            _ => Low,
        };
    }

    private static void UpdateEnvironmentFrom(Environment env, VideoQuality quality)
    {   
        SsrQuality settings = From(quality);

        if (quality == VideoQuality.Disabled)
        {
            env.SsrEnabled = false;
            return;
        }

        env.SsrEnabled = true;

        env.SsrMaxSteps = settings.MaxSteps;
    }

    public static void UpdateEnvironment(Environment env)
        => UpdateEnvironmentFrom(env, Quality);

    private readonly static SsrQuality Low     = new(32);
    private readonly static SsrQuality Medium  = new(64);
    private readonly static SsrQuality High    = new(128);
    private readonly static SsrQuality Ultra   = new(256);
}

public record SsrQuality(
    int MaxSteps);