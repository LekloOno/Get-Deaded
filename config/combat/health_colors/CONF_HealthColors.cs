using Godot;

namespace Pew;

public partial class CONF_HealthColors : Node
{
    public static CONF_HealthColors Instance { get; private set; }

    public CONFD_HealthColors Bars { get; private set; }
    public CONFD_DamageColors Damages { get; private set; }

    public override void _EnterTree()
    {
        Instance = this;
        Bars = (CONFD_HealthColors) ResourceLoader.Load("res://config/combat/health_colors/conf_health_colors.tres");
        Damages = (CONFD_DamageColors) ResourceLoader.Load("res://config/combat/health_colors/conf_damage_colors.tres");
    }

    public static CONFD_BarColors GetBarColors(GC_Health healthType)
    {
        return healthType switch
        {
            GC_Armor _ =>   Instance.Bars.Armor,
            GC_Shield _ =>  Instance.Bars.Shield,
            GC_SpeedShield _ => Instance.Bars.Shield,
            GC_Barrier _ => Instance.Bars.Barrier,
            GC_Health _ =>  Instance.Bars.Health,
            _ => Instance.Bars.Default,
        };
    }

    public static Color GetDamageColors(GC_Health healthType)
    {
        return healthType switch
        {
            GC_Armor _ =>   Instance.Damages.Armor,
            GC_Shield _ =>  Instance.Damages.Shield,
            GC_SpeedShield _ => Instance.Damages.Shield,
            GC_Barrier _ => Instance.Damages.Barrier,
            GC_Health _ =>  Instance.Damages.Health,
            _ => Instance.Damages.Default,
        };
    }
}