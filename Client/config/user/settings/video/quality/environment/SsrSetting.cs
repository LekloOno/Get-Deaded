using Godot;

public partial class SsrSetting : VideoQualitySetting
{
    public const string KeyString = "screen_space_reflection";
    public override string Key => KeyString;

    protected override void UpdateFrom(VideoQuality quality)
    {
        Environment? env = GetViewport()?.World3D?.Environment;

        if (env == null)
        {
            GD.PrintErr("[SsrSetting] no Environment found.");
            return;
        }

        UpdateEnvironment(env);
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

    public static void UpdateEnvironment(Environment env)
    {
        SsrQuality settings = From(Quality);

        if (Quality == VideoQuality.Disabled)
        {
            env.SsrEnabled = false;
            GD.Print("disable");
            return;
        }

        env.SsrEnabled = true;

        env.SsrMaxSteps = settings.MaxSteps;
    }

    private readonly static SsrQuality Low     = new(32);
    private readonly static SsrQuality Medium  = new(64);
    private readonly static SsrQuality High    = new(128);
    private readonly static SsrQuality Ultra   = new(256);
}

public record SsrQuality(
    int MaxSteps);