using Godot;

[GlobalClass]
public partial class GB_CharacterBodyManager : GB_ExternalBodyManagerWrapper
{
    [Export] private GB_CharacterBody _body = null!;

    public override GB_IExternalBodyManager Body => _body;
}