using Godot;

public partial class GlowSetting : VideoQualitySetting
{
    public const string KeyString = "glow_quality";
    public override string Key => KeyString;
    protected override void UpdateFrom(VideoQuality quality, out VideoQuality effectiveQuality)
    {
        Environment? env = GetViewport()?.World3D?.Environment;

        if (quality == VideoQuality.Disabled)
            effectiveQuality = VideoQuality.Minimal;
        else
            effectiveQuality = quality;

        if (env == null)
        {
            GD.PrintErr("[GlowSetting] no Environment found.");
            return;
        }

        UpdateEnvironmentFrom(env, effectiveQuality);
    }
    
    private static GlowQuality From(VideoQuality quality)
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
        GlowQuality settings = From(quality);

        RenderingServer.EnvironmentGlowSetUseBicubicUpscale(settings.BicubicUpScale);

        for (int i = 0; i < 7; i++)
            env.SetGlowLevel(i, settings.Levels[i]);
    }

    public static void UpdateEnvironment(Environment env)
        => UpdateEnvironmentFrom(env, Quality);

    private readonly static GlowQuality Minimal = new(false, [0.0f, 1.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f]);
    private readonly static GlowQuality Low     = new(false, [0.0f, 1.0f, 0.5f, 0.0f, 0.0f, 0.0f, 0.0f]);
    private readonly static GlowQuality Medium  = new(false, [0.0f, 0.8f, 0.4f, 0.1f, 0.0f, 0.0f, 0.0f]);
    private readonly static GlowQuality High    = new(true,  [0.0f, 0.8f, 0.4f, 0.1f, 0.1f, 0.0f, 0.0f]);
    private readonly static GlowQuality Ultra   = new(true,  [0.0f, 0.8f, 0.4f, 0.1f, 0.1f, 0.1f, 0.1f]);
}

public record GlowQuality(
    bool BicubicUpScale,
    float[] Levels);