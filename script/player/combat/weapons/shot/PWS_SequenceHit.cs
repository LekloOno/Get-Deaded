using Godot;

[GlobalClass]
public partial class PWS_SequenceHit : PW_Shot
{
    [Export] private GC_SequenceHitBox _sequenceHitBox;

    public override void SpecInitialize(GB_ExternalBodyManagerWrapper ownerBody)
    {
        _sequenceHitBox.CollisionMask = CONF_Collision.Masks.HitScan;
        _sequenceHitBox.CollisionLayer = 0;
        _sequenceHitBox.HitHurtBox += DoHitEntity;
        _sequenceHitBox.HitEnvironment += DoHitEnvironment;
    }

    private void DoHitEnvironment() =>
        HandleKick(Direction);

    private void DoHitEntity(GC_HurtBox hurtBox)
    {
        Vector3 hitPosition = ApproxHitPosition(CONF_Collision.Layers.EnnemiesHurtBox, hurtBox.GlobalPosition);

        SendHit(hurtBox, hitPosition, GlobalPosition, Direction);
        HandleKick(Direction);
    }

    private Vector3 ApproxHitPosition(uint targetMask, Vector3 targetHitPosition)
    {
        PhysicsDirectSpaceState3D spaceState = GetWorld3D().DirectSpaceState;
        PhysicsRayQueryParameters3D query = PhysicsRayQueryParameters3D.Create(GlobalPosition, targetHitPosition);
        query.CollideWithAreas = true;
        query.CollisionMask = targetMask;

        var result = spaceState.IntersectRay(query);

        if (result.Count > 0)
            return (Vector3)result["position"];

        return targetHitPosition;
    }

    protected override void ShootWithSpread(Vector3 direction) =>
        _sequenceHitBox.StartSequence();

    public override void Interrupt() =>
        _sequenceHitBox.StopSequence();

    public override void Sleep() =>
        _sequenceHitBox.CollisionMask = 0;

    public override void Awake() =>
        _sequenceHitBox.CollisionMask = CONF_Collision.Masks.HitScan;
}