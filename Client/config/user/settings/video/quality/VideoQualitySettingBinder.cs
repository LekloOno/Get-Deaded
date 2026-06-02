using Godot;

public partial class VideoQualitySettingBinder : Node
{
    [Export] private WorldEnvironment? _environment;
    [Export] private DirectionalLight3D? _mainLight;

    public override void _Ready()
    {
        if (_mainLight != null)
            DirectionalShadowsSetting.UpdateLight(_mainLight);

        if (_environment != null)
        {
            AmbientOcclusionSetting.UpdateEnvironment(_environment.Environment);
            SsilSetting.UpdateEnvironment(_environment.Environment);
            GlowSetting.UpdateEnvironment(_environment.Environment);
        }
    }
}