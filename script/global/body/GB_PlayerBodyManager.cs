using Godot;

[GlobalClass]
public partial class GB_PlayerBodyManager : GB_ExternalBodyManager
{
    [Export] private PM_Controller _body;
    public override void HandleKnockBack(Vector3 force)
    {
        _body.AdditionalForces.AddImpulse(force);
    }

    public override Vector3 Velocity() => _body.RealVelocity;
}