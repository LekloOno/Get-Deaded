using Godot;

[GlobalClass]
public partial class GB_CharacterBodyManager : GB_ExternalBodyManagerWrapper
{
    [Export] public GB_CharacterBody CharacterBody {get; private set;} = null!;

    public override GB_IExternalBodyManager Body => CharacterBody;
}