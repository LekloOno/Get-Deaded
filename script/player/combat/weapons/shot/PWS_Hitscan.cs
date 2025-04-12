using System;
using Godot;

[GlobalClass]
public partial class PWS_Hitscan : PW_Shot, GC_IHitDealer
{
    [Export] protected float _maxDistance = 20f;

    public override void Shoot(Vector3 origin, Vector3 direction)
    {
        Vector3 castOrigin = origin + _originOffset;
        Vector3 castDirection = direction + _directionOffset;

        Vector3 hit = castOrigin + castDirection * _maxDistance;

        World3D world = _barrel.GetWorld3D();
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
            trail.Shoot(_barrel, _barrel.GlobalPosition + _originOffset, hit);

        HandleKick(origin, direction);
    }

    public override void SpecInitialize(Node3D barel, GB_ExternalBodyManager ownerBody)
    {
        if (_maxDistance == 0)
        {
            _maxDistance = 0.1f;
            throw new InvalidOperationException($"PWS_Hitscan._maxDistance cannot be 0, it has been defaulted to {_maxDistance}.");
        }
    }
}