using Godot;

[GlobalClass]
public partial class GE_EnemyCombatWrapper : GE_CombatWrapper
{
    [Export] private E_Enemy _enemy;

    public override GC_HealthManager HealthManager => _enemy.HealthManager;
    public override GB_IExternalBodyManager Body => _enemy.Body;
}