using Godot;

[GlobalClass]
public partial class GB_PlayerBodyManager : GB_ExternalBodyManagerWrapper
{
    [Export] private PM_Controller _body;

    public override GB_IExternalBodyManager Body => _body;
}