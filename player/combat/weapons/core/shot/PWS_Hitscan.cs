using Godot;

[GlobalClass]
public partial class PWS_Hitscan : PW_Shot
{
    [Export] protected float _maxDistance;
    [Export] protected VFX_HitscanTrail _trail;
    public override void HandleShoot(Node3D manager, Vector3 origin, Vector3 direction)
    {
        Vector3 castOrigin = origin + _originOffset;
        Vector3 castDirection = direction + _directionOffset;

        Vector3 hit = castOrigin + castDirection * _maxDistance;

        World3D world = manager.GetWorld3D();
        PhysicsDirectSpaceState3D spaceState = world.DirectSpaceState;
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(castOrigin, hit);
        query.CollideWithAreas = true;
        query.CollisionMask = 2;

        var result = spaceState.IntersectRay(query);

        if (result.Count > 0)
        {
            hit = (Vector3)result["position"];
            Node3D collider = result["collider"].AsGodotObject() as Node3D;
            if (collider is GC_HurtBox hurtBox)
            {
                bool killed = _hitData.SendHit(hurtBox, out float takenDamage);
                Hit?.Invoke(this, new(hurtBox.HealthManager, hurtBox.HealthManager.GetExposedLayer(), hurtBox, takenDamage, killed));
            }
            else
            {
                Hit?.Invoke(this, ShotHitEventArgs.MISS);
            }
        }

        _trail?.Shoot(manager, _barel.GlobalPosition + _originOffset, hit);
    }
}