using Godot;

[GlobalClass]
public partial class PAF_GenericBis : PA_FireBis
{
    [Export] private PW_FireBis _fire;

    public override PW_FireBis Fire => _fire;
}