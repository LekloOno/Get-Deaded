using Godot;

[GlobalClass]
public partial class PWS_SequenceHit : PW_Shot
{
    [Export] private GC_SequenceHitBox _sequenceHitBox;

    public override void SpecInitialize(GB_ExternalBodyManagerWrapper ownerBody)
    {
        _sequenceHitBox.CollisionMask = CONF_Collision.Masks.HitScan;
        _sequenceHitBox.CollisionLayer = 0;
        _sequenceHitBox.HitEntity += DoHitEntity;
    }

    private void DoHitEntity(GC_HurtBox hurtBox)
    {
        Vector3 castOrigin = OwnerEntity.Body.GlobalTransform.Origin;
        Vector3 hitPosition = hurtBox.GlobalPosition;

        PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(castOrigin, hitPosition);
        query.CollideWithAreas = true;
        query.CollisionMask = CONF_Collision.Masks.HitBox;

        var result = spaceState.IntersectRay(query);

        if (result.Count > 0)
            hitPosition = (Vector3)result["position"];

        SendHit(hurtBox, hitPosition, castOrigin);
    }

    protected override void ShootWithSpread(Vector3 direction) =>
        _sequenceHitBox.StartSequence();

    public override void Interrupt() =>
        _sequenceHitBox.StopSequence();
}