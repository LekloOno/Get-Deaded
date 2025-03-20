using Godot;

[GlobalClass]
public partial class PWF_Ponctual : PW_Fire
{
    protected override bool Press() => TryShoot();
    protected override bool Release() => true;
    public override void Disable() {}
}