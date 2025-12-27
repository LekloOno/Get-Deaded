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
            // To avoid hitting multiple times the single target, yet allow hitting multiple different ones.
            HashSet<GE_ICombatEntity> _hitEntities = [];

            for (int i = 0; i < _shapeCast.GetCollisionCount(); i ++)
            {
                Vector3 hitPosition = _shapeCast.GetCollisionPoint(i);
                Node3D collider = _shapeCast.GetCollider(i) as Node3D;

                if (collider is GC_HurtBox hurtBox)
                {
                    if (!_hitEntities.Add(hurtBox.Entity))
                        continue;

                    SendHit(hurtBox, hitPosition, castOrigin, castDirection);
                }
                else
                    DoHit(HitEventArgs.Miss(this, OwnerEntity), hitPosition);
            }
            
            _trail?.Shoot(GlobalPosition + _shapeCast.TargetPosition.Length() * direction.Normalized());
            HandleKick(castDirection);
        }
    }
}