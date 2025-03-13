using Godot;

[GlobalClass]
public partial class PW_SimpleClick : PW_Hitscan
{
    public override void Shoot(Node3D sight)
    {
        GC_HurtBox hurtBox = Hit(sight);
        if (hurtBox == null)
            return;

        GD.Print("caca");
        hurtBox.Damage(_damage);
    }
}