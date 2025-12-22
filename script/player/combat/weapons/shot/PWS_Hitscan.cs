using System;
using Godot;

namespace Pew;

[GlobalClass]
public partial class PWS_Hitscan : PW_Shot, GC_IHitDealer
{
    [Export] protected float _maxDistance = 20f;

    protected override void ShootWithSpread(Vector3 direction)
    {
        Vector3 castOrigin = GlobalPosition;
        Vector3 castDirection = direction;

        Vector3 hitPosition = castOrigin + castDirection * _maxDistance;

        World3D world = GetWorld3D();
        PhysicsDirectSpaceState3D spaceState = world.DirectSpaceState;
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(castOrigin, hitPosition);
        query.CollideWithAreas = true;
        query.CollisionMask = CONF_Collision.Masks.HitScan;

        var result = spaceState.IntersectRay(query);

        if (result.Count > 0)
        {
            hitPosition = (Vector3)result["position"];
            Node3D collider = result["collider"].AsGodotObject() as Node3D;

            if (collider is GC_HurtBox hurtBox)
                SendHit(hurtBox, hitPosition, castOrigin);
            else
                DoHit(HitEventArgs.Miss(this, OwnerEntity), hitPosition);
        }

        _trail?.Shoot(hitPosition);
        HandleKick(castDirection);
    }

    public override void SpecInitialize(GB_ExternalBodyManagerWrapper ownerBody)
    {
        if (_maxDistance == 0)
        {
            _maxDistance = 0.1f;
            throw new InvalidOperationException($"PWS_Hitscan._maxDistance cannot be 0, it has been defaulted to {_maxDistance}.");
        }
    }
}