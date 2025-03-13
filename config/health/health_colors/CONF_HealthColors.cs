using Godot;

public partial class CONF_HealthColors : Node
{
    public static CONF_HealthColors Instance { get; private set; }

    public DATA_HealthColors Config { get; private set; }

    public override void _EnterTree()
    {
        Instance = this;
        Config = (DATA_HealthColors) ResourceLoader.Load("res://config/health/health_colors/conf_health.colors.tres");
    }
}