using System;
using Godot;

[GlobalClass]
public abstract partial class PW_Shot : Resource, GC_IHitDealer
{
    [Export] protected GC_Hit _hitData;
    [Export] protected Vector3 _originOffset = Vector3.Zero;
    [Export] protected Vector3 _directionOffset = Vector3.Zero;
    [Export] protected Godot.Collections.Array<VFX_Trail> _trails;
    protected Node3D _barel;

    public EventHandler<ShotHitEventArgs> Hit;

    GC_Hit GC_IHitDealer.HitData => _hitData;

    public void Initialize(Node3D barel)
    {
        _barel = barel;
        _hitData.InitializeModifiers();
    }    

    //public void Shoot(Vector3 origin, Vector3 direction) =>
    //    HShoot(origin, direction);

    public abstract void Shoot(Vector3 origin, Vector3 direction);

    protected void DoHit(ShotHitEventArgs e, Vector3 hitPosition, Vector3 from)
    {
        Hit?.Invoke(this, e);
        e.HurtBox?.TriggerDamageParticles(hitPosition, from);
    }
}