using Godot;

[GlobalClass]
public partial class PAF_Generic : PA_Fire
{
    [Export] private PW_Fire _fire;

    public override PW_Fire Fire => _fire;
}