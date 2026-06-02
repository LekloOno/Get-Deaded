using Godot;

public partial class VideoQualitySettingBinder : Node
{
    [Export] private WorldEnvironment? _environment;
    [Export] private DirectionalLight3D? _terrainLight;
    [Export] private DirectionalLight3D? _dynamicLight;

    public override void _Ready()
    {
        if (_dynamicLight != null)
            DirectionalShadowsSetting.UpdateLight(_dynamicLight);

        if (_terrainLight != null)
            DirectionalShadowsSetting.UpdateLight(_terrainLight);

        if (_environment != null)
        {
            AmbientOcclusionSetting.UpdateEnvironment(_environment.Environment);
            SsilSetting.UpdateEnvironment(_environment.Environment);
            GlowSetting.UpdateEnvironment(_environment.Environment);
            SsrSetting.UpdateEnvironment(_environment.Environment);
        }
    }
}