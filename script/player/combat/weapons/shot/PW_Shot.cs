using System;
using Godot;

// Icon credits - under CC BY 4.0 - https://www.onlinewebfonts.com/icon/504938
[GlobalClass, Icon("res://gd_icons/weapon_system/shot_icon.svg")]
public abstract partial class PW_Shot : WeaponSystem, GC_IHitDealer
{
    [Export] protected GC_Hit _hitData;
    [Export] protected Vector3 _originOffset = Vector3.Zero;
    [Export] protected Vector3 _directionOffset = Vector3.Zero;
    [Export] protected float _knockBack = 0f;
    [Export] private float _kickBack = 0f;

    [ExportCategory("Visuals")]
    [Export] protected Godot.Collections.Array<VFX_Trail> _trails;
    [Export] private PCT_UndirectScalable _traumaCauser;
    [Export] private bool _clampTrauma = true;
    [Export] private float _maxTrauma = 0.2f;

    private GB_ExternalBodyManager _ownerBody;
    protected Node3D _barrel;

    public GC_Hit HitData => _hitData;
    public EventHandler<ShotHitEventArgs> Hit;

    public void Initialize(Node3D barel, GB_ExternalBodyManager ownerBody)
    {
        _barrel = barel;
        _ownerBody = ownerBody;

        _hitData.InitializeModifiers();

        SpecInitialize(barel, ownerBody);
    }

    public abstract void SpecInitialize(Node3D barel, GB_ExternalBodyManager ownerBody);

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

        // OPTIMIZE ME - Reduces editability
        // We could generate one specialization of trauma causer which directly does Clamped or not clamped. 
        if (_clampTrauma)
            _traumaCauser.CauseClampedTrauma(_maxTrauma);
        else
            _traumaCauser.CauseTrauma();
    }
}