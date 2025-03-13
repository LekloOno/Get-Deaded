using Godot;

public partial class PW_Hitscan : PW_Weapon
{
    [Export] protected float _maxDistance;

    protected GC_HurtBox Hit(Node3D sight)
    {
        SightTo(sight, out Vector3 origin, out Vector3 direction);

        PhysicsDirectSpaceState3D spaceState = sight.GetWorld3D().DirectSpaceState;
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(origin, origin + direction * _maxDistance);
        query.CollideWithAreas = true;
        query.CollisionMask = 2;

        var result = spaceState.IntersectRay(query);

        if (result.Count > 0)
        {
            Node3D collider = result["collider"].AsGodotObject() as Node3D;
            if (collider is GC_HurtBox hurtBox)
                return hurtBox;
        }

        return null;
    }
}