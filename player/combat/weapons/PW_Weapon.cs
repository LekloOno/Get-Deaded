using Godot;

[GlobalClass]
public partial class PW_Weapon : Resource
{
    [Export] protected GC_Hit _hit;
    public virtual void Shoot(Node3D sight)
    {
        GD.Print("pew ! ");
    }

    protected void SightTo(Node3D sight, out Vector3 origin, out Vector3 direction)
    {
        origin = sight.GlobalPosition;
        direction = -sight.GlobalBasis.Z;
    }
}