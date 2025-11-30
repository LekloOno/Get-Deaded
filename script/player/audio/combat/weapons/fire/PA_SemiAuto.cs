using Godot;

[GlobalClass]
public partial class PA_SemiAuto : PA_Fire
{
    [Export] private PWF_SemiAuto _fire;

    public override PW_Fire Fire => _fire;
}