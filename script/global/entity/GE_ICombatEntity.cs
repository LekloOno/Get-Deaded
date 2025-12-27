/// <summary>
/// Describes the base components of an entity that can be evolved in any form of "combat".
/// </summary>
public interface GE_ICombatEntity : GE_Entity
{
    public GC_HealthManager HealthManager {get;}
}