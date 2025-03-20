using Godot;

[GlobalClass]
public partial class PWF_Ponctual : PW_Fire
{
    public override void Press() => TryShoot();
    public override void Release() {}
}