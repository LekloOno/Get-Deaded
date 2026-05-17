using Godot;

[GlobalClass]
public abstract partial class GE_CombatEntity : Node, GE_ICombatEntity
{
    public abstract GC_HealthManager HealthManager {get;}
    public abstract GB_IExternalBodyManager Body {get;}
    public abstract PCT_SimpleTraumaData KillTraumaData {get;}
}