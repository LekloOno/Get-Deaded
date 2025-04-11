using Godot;

[GlobalClass]
public partial class PWS_Hitscan : PW_Shot, GC_IHitDealer
{
    [Export] protected float _maxDistance;

    public override void Shoot(Vector3 origin, Vector3 direction)
    {
        Vector3 castOrigin = origin + _originOffset;
        Vector3 castDirection = direction + _directionOffset;

        Vector3 hit = castOrigin + castDirection * _maxDistance;

        World3D world = _barel.GetWorld3D();
        PhysicsDirectSpaceState3D spaceState = world.DirectSpaceState;
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(castOrigin, hit);
        query.CollideWithAreas = true;
        query.CollisionMask = CONF_Collision.Masks.HitScan;

        var result = spaceState.IntersectRay(query);

        if (result.Count > 0)
        {
            hit = (Vector3)result["position"];
            Node3D collider = result["collider"].AsGodotObject() as Node3D;
            if (collider is GC_HurtBox hurtBox)
            {
                hurtBox.HandleKnockBack(_knockBack * castDirection.Normalized());
                bool killed = hurtBox.Damage(this, out float takenDamage);
                DoHit(new(hurtBox.HealthManager, hurtBox.HealthManager.GetExposedLayer(), hurtBox, takenDamage, killed), hit, castOrigin);
            }
            else
            {
                DoHit(ShotHitEventArgs.MISS, hit, castOrigin);
            }
        }

        foreach (VFX_Trail trail in _trails)
            trail.Shoot(_barel, _barel.GlobalPosition + _originOffset, hit);

        HandleKick(origin, direction);
    }

    public override void ShotInitialize(){}
}