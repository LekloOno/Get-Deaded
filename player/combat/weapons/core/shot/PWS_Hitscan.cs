using Godot;

[GlobalClass]
public partial class PWS_Hitscan : PW_Shot
{
    [Export] protected float _maxDistance;
    public override void HandleShoot(World3D world, Vector3 origin, Vector3 direction)
    {
        PhysicsDirectSpaceState3D spaceState = world.DirectSpaceState;
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(origin, origin + direction * _maxDistance);
        query.CollideWithAreas = true;
        query.CollisionMask = 2;

        var result = spaceState.IntersectRay(query);


        if (result.Count > 0)
        {
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
    }
}