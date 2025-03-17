using Godot;

public partial class PWF_Ponctual : PW_Fire
{
    public override void Press()
    {
        if (CanShoot())
            Shoot();
    }
    public override void Release() {}
}