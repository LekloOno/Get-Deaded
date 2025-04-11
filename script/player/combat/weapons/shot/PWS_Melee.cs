using Godot;

public partial class PWS_Melee : PW_Shot
{
    [Export] private PackedScene _meleeHitBox;
    private PWS_MeleeHitBox _area;

    public override void Shoot(Vector3 origin, Vector3 direction)
    {
        _area?.QueueFree();
        _area = (PWS_MeleeHitBox) _meleeHitBox.Instantiate();
    }

    public override void ShotInitialize()
    {
        _area = (PWS_MeleeHitBox) _meleeHitBox.Instantiate();
        _barel.AddChild(_area);
    }
}