using Godot;

public partial class GlowSetting : VideoQualitySetting
{
    public override string Key => "glow_quality";

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

    public static void UpdateEnvironment(Environment env)
    {
        GlowQuality settings = From(Quality);

        RenderingServer.EnvironmentGlowSetUseBicubicUpscale(settings.BicubicUpScale);

        for (int i = 0; i < 7; i++)
            env.SetGlowLevel(i, settings.Levels[i]);
    }

    private readonly static GlowQuality Minimal = new(false, [0.0f, 1.5f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f]);
    private readonly static GlowQuality Low     = new(false, [0.0f, 1.0f, 0.5f, 0.0f, 0.0f, 0.0f, 0.0f]);
    private readonly static GlowQuality Medium  = new(false, [0.0f, 0.8f, 0.4f, 0.1f, 0.0f, 0.0f, 0.0f]);
    private readonly static GlowQuality High    = new(true,  [0.0f, 0.8f, 0.4f, 0.1f, 0.1f, 0.0f, 0.0f]);
    private readonly static GlowQuality Ultra   = new(true,  [0.0f, 0.8f, 0.4f, 0.1f, 0.1f, 0.1f, 0.1f]);
}

public record GlowQuality(
    bool BicubicUpScale,
    float[] Levels);