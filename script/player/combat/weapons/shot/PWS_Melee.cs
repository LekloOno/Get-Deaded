using Godot;

public partial class PWS_Melee : PW_Shot
{
    [Export] private PackedScene _meleeHitBox;
    private PWS_MeleeHitBox _area;

    public override void SpecInitialize(GB_ExternalBodyManager ownerBody)
    {
        _area = (PWS_MeleeHitBox) _meleeHitBox.Instantiate();
        //_barrel.AddChild(_area);
    }

    protected override void ShootWithSpread(Vector3 direction)
    {
        _area?.QueueFree();
        _area = (PWS_MeleeHitBox) _meleeHitBox.Instantiate();
    }

}