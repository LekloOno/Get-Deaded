using Godot;

[GlobalClass]
public partial class GE_PlayerCombatWrapper : GE_ActiveCombatEntity
{
    [Export] private PM_Controller _controller;

    public override GC_HealthManager HealthManager => _controller.HealthManager;
    public override GB_IExternalBodyManager Body => _controller.Body;
    public override PW_WeaponsHandler WeaponsHandler => _controller.WeaponsHandler;
    public override PCT_SimpleTraumaData KillTraumaData => _controller.KillTraumaData;
}