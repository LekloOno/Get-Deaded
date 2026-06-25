using Godot;

[GlobalClass]
public partial class E_FreezerCombatWrapper : GE_ActiveCombatEntity
{
    [Export] private E_Freezer _freezer = null!;

    public override GC_HealthManager HealthManager => _freezer.HealthManager;
    public override GB_IExternalBodyManager Body => _freezer.Body;
    public override PW_WeaponsHandler WeaponsHandler => _freezer.WeaponsHandler;
    public override PCT_SimpleTraumaData KillTraumaData => _freezer.KillTraumaData;
    public override bool Alive => _freezer.Alive;
}