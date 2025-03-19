using System;
using Godot;

[GlobalClass]
public abstract partial class PW_Shot : Resource
{
    [Export] protected GC_Hit _hitData;
    [Export] protected Vector3 _originOffset = Vector3.Zero;
    [Export] protected Vector3 _directionOffset = Vector3.Zero;
    protected Node3D _barel;

    public EventHandler<ShotHitEventArgs> Hit;

    public void Initialize(Node3D barel)
    {
        _barel = barel;
        _hitData.InitializeModifiers();
    }    

    public void Shoot(Node3D manager, Vector3 origin, Vector3 direction) =>
        HandleShoot(manager, origin, direction);

    public abstract void HandleShoot(Node3D manager, Vector3 origin, Vector3 direction);

    protected void DoHit(ShotHitEventArgs e, Vector3 hitPosition, Vector3 from)
    {
        Hit?.Invoke(this, e);
        e.HurtBox?.TriggerDamageParticles(hitPosition, from);
    }
}