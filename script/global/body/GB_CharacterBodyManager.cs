using Godot;

[GlobalClass]
public partial class GB_CharacterBodyManager : GB_ExternalBodyManager
{
    [Export] private CharacterBody3D _body;
    public override void HandleKnockBack(Vector3 force)
    {
        _body.Velocity += force;
    }

    public override Vector3 Velocity() => _body.Velocity;
}