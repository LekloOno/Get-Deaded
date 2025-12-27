using Godot;

[GlobalClass]
public partial class GE_EnemyCombatWrapper : GE_ActiveCombatEntity
{
    [Export] private E_Enemy _enemy;

    public override GC_HealthManager HealthManager => _enemy.HealthManager;
    public override GB_IExternalBodyManager Body => _enemy.Body;
    public override PW_WeaponsHandler WeaponsHandler => _enemy.WeaponsHandler;
    public override PCT_SimpleTraumaData KillTraumaData => _enemy.KillTraumaData;
}