using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class PWS_ShapeScan : PW_Shot
{
    [Export] private ShapeCast3D _shapeCast;

    public override void SpecInitialize(GB_ExternalBodyManagerWrapper ownerBody)
    {
        _shapeCast.CollisionMask = CONF_Collision.Masks.HitScan;
        _shapeCast.CollideWithAreas = true;
    }

    protected override void ShootWithSpread(Vector3 direction)
    {
        Vector3 castOrigin = GlobalPosition;
        Vector3 castDirection = direction;

        _shapeCast.ForceShapecastUpdate();
        if (_shapeCast.IsColliding())
        {
            List<GC_HealthManager> _healthManagers = [];
            for (int i = 0; i < _shapeCast.GetCollisionCount(); i ++)
            {
                Vector3 hit = _shapeCast.GetCollisionPoint(i);
                Node3D collider = _shapeCast.GetCollider(i) as Node3D;

                if (collider is GC_HurtBox hurtBox)
                {
                    if (_healthManagers.Contains(hurtBox.HealthManager))
                        continue;

                    _healthManagers.Add(hurtBox.HealthManager);

                    hurtBox.HandleKnockBack(KnockBackFrom(castDirection));
                    bool killed = hurtBox.Damage(this, out float takenDamage, out float overflow);

                    DoHit(
                        new(
                            hurtBox.HealthManager,
                            hurtBox.HealthManager.GetExposedLayer(),
                            hurtBox,
                            takenDamage,
                            killed,
                            overflow,
                            _ignoreCrit
                        ),
                        hit,
                        castOrigin
                    );
                }
                else
                {
                    DoHit(ShotHitEventArgs.MISS, hit, castOrigin);
                }
            }

            _trail?.Shoot(GlobalPosition + _shapeCast.TargetPosition.Length() * direction.Normalized());
            HandleKick(castOrigin, castDirection);
        }


    }

}