using Godot;

[GlobalClass]
public abstract partial class GE_ActiveCombatEntity : GE_CombatEntity, GE_IActiveCombatEntity
{
    public abstract PW_WeaponsHandler WeaponsHandler {get;}
}