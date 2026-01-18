using Godot;

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

    public static CONFD_IBarColors GetBarColors(GC_Health healthType, CONF_HurtBoxFaction faction = CONF_HurtBoxFaction.Enemy)
    {
        return healthType switch
        {
            GC_Armor _ =>   Instance.Bars.Armor,
            GC_Shield _ =>  Instance.Bars.Shield,
            GC_SpeedShield _ => Instance.Bars.Shield,
            GC_Barrier _ => Instance.Bars.Barrier,
            GC_Health _ => GetDefaultBarColors(faction),
            _ => Instance.Bars.Default,
        };
    }

    public static CONFD_IBarColors GetDefaultBarColors(CONF_HurtBoxFaction faction)
    {
        if (!Instance.Bars.InvertEnemyColors)
            return CONFD_DefaultBarColors.Default;
            
        return faction switch
        {
            CONF_HurtBoxFaction.Player => CONFD_DefaultBarColors.Default,
            CONF_HurtBoxFaction.Enemy => CONFD_DefaultHostileBarColors.Default,
            _ => CONFD_DefaultHostileBarColors.Default,

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