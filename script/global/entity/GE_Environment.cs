namespace Pew;

/// <summary>
/// Represents the environment as an entity. <br/>
/// For example, if an enemy dies in lava, out of a fall, void, etc., the entity credited for the kill would be the environment.
/// </summary>
public class GE_Environment : GE_IActiveCombatEntity
{
    public static readonly GE_Environment Instance = new();
    

    // Maybe later improve with null object for the members too.
    public GC_HealthManager HealthManager => null;

    public PW_WeaponsHandler WeaponsHandler => null;

    public GB_IExternalBodyManager Body => null;
}