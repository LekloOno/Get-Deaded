using Godot;

[GlobalClass]
public partial class PW_Weapon : Resource
{
    [Export] protected float _damage;
    public virtual void Shoot(Node3D sight)
    {
        GD.Print("pew ! " + _damage);
    }

    protected void SightTo(Node3D sight, out Vector3 origin, out Vector3 direction)
    {
        origin = sight.GlobalPosition;
        direction = -sight.GlobalBasis.Z;
    }
}