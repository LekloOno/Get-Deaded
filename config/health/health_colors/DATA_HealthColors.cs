using Godot;

[GlobalClass]
public partial class DATA_HealthColors : Resource
{   
    [Export] public DATA_BarColors Health;
    [Export] public DATA_BarColors Shield;
    [Export] public DATA_BarColors Barrier;
    [Export] public DATA_BarColors Armor;
    [Export] public DATA_BarColors Default;

    public static DATA_BarColors GetBarColors(GC_Health healthType)
    {
        return healthType switch
        {
            GC_Armor _ =>   CONF_HealthColors.Instance.Config.Health,
            GC_Shield _ =>   CONF_HealthColors.Instance.Config.Shield,
            GC_Health _ =>  CONF_HealthColors.Instance.Config.Barrier,
            _ => CONF_HealthColors.Instance.Config.Default,
        };
    }
}