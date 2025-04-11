using System;
using Godot;

[GlobalClass]
public abstract partial class PW_Shot : Resource, GC_IHitDealer
{
    [Export] protected GC_Hit _hitData;
    [Export] protected Vector3 _originOffset = Vector3.Zero;
    [Export] protected Vector3 _directionOffset = Vector3.Zero;
    [Export] protected Godot.Collections.Array<VFX_Trail> _trails;
    [Export] protected float _knockBack = 0f;
    [Export] private float _kickBack = 0f;
    [Export] private MATH_FloatCurveSampler _traumaSampler;
    [Export] private float _traumaRadius = 1f;
    [Export] private bool _clampTrauma = true;
    [Export] private float _maxTrauma = 0.2f;

    private PCT_UndirectScalable _traumaCauser;
    private GB_ExternalBodyManager _ownerBody;
    protected Node3D _barel;

    public EventHandler<ShotHitEventArgs> Hit;

    GC_Hit GC_IHitDealer.HitData => _hitData;

    public void Initialize(Node3D barel, GB_ExternalBodyManager ownerBody)
    {
        _barel = barel;
        _ownerBody = ownerBody;
        _hitData.InitializeModifiers();

        if (_traumaSampler == null)
            return;

        if (_traumaCauser != null)
            _traumaCauser.QueueFree();
        
        _traumaCauser = new(_traumaSampler);
        CollisionShape3D causerShape = new(){Shape = new SphereShape3D(){Radius = _traumaRadius}};
        _traumaCauser.AddChild(causerShape);

        _barel.AddChild(_traumaCauser);
        ShotInitialize();
    }

    public abstract void ShotInitialize();    

    //public void Shoot(Vector3 origin, Vector3 direction) =>
    //    HShoot(origin, direction);

    public abstract void Shoot(Vector3 origin, Vector3 direction);

    protected void HandleKick(Vector3 origin, Vector3 direction)
    {
        if (_kickBack != 0)
            _ownerBody.HandleKnockBack(-direction * _kickBack);
    }

    protected void DoHit(ShotHitEventArgs e, Vector3 hitPosition, Vector3 from)
    {
        Hit?.Invoke(this, e);
        e.HurtBox?.TriggerDamageParticles(hitPosition, from);
        
        if (_traumaCauser == null)
            return;

        _traumaCauser.GlobalPosition = hitPosition;

        if (_clampTrauma)
            _traumaCauser.CauseClampedTrauma(_maxTrauma);
        else
            _traumaCauser.CauseTrauma();
    }
}