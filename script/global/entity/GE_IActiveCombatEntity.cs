using System;

/// <summary>
/// Describes the base components of an entity that can be evolved in any form of active "combat". <br/>
/// That is, it can be attacked, but can also attack.
/// </summary>
public interface GE_IActiveCombatEntity : GE_ICombatEntity
{
    public PW_WeaponsHandler WeaponsHandler {get;}
}