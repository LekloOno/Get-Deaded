using Godot;

[GlobalClass]
public partial class GE_PlayerCombatWrapper : GE_CombatWrapper
{
    [Export] private PM_Controller _controller;

    public override GC_HealthManager HealthManager => _controller.HealthManager;
    public override GB_IExternalBodyManager Body => _controller.Body;
}