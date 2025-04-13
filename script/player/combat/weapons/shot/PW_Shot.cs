using System;
using Godot;

/// <summary>
/// Handles final shot outcomes - shooting.
/// <para>
/// Its GlobalPosition and -GlobalBasis.Z serves as the shooting origin position and direction.<br/>
/// You can offset it from the weapon transform to create specific patterns.
/// </para>
/// </summary>

// Icon credits - under CC BY 4.0 - https://www.onlinewebfonts.com/icon/504938
[GlobalClass, Icon("res://gd_icons/weapon_system/shot_icon.svg")]
public abstract partial class PW_Shot : WeaponComponent, GC_IHitDealer
{
    [Export] protected GC_Hit _hitData;
    [Export] protected Vector3 _originOffset = Vector3.Zero;
    [Export] protected Vector3 _directionOffset = Vector3.Zero;
    [Export] protected float _spread = 0f;          // In degrees
    [Export] protected float _knockBack = 0f;
    [Export] private float _kickBack = 0f;

    [ExportCategory("Visuals")]
    [Export] protected PW_Trail _trail;
    [Export] private PCT_UndirectScalable _traumaCauser;
    [Export] private bool _clampTrauma = true;
    [Export] private float _maxTrauma = 0.2f;

    private GB_ExternalBodyManager _ownerBody;

    public EventHandler<ShotHitEventArgs> Hit;
    public MATH_AdditiveModifiers SpreadMultiplier {get; private set;} = new();
    public GC_Hit HitData => _hitData;
    public Vector3 Direction => -GlobalBasis.Z; 
    private static Random _random = new();

    public void Initialize(GB_ExternalBodyManager ownerBody)
    {
        _ownerBody = ownerBody;
        _hitData.InitializeModifiers();
        SpecInitialize(ownerBody);
    }

    public abstract void SpecInitialize(GB_ExternalBodyManager ownerBody);

    public void Shoot()
    {
        Vector3 direction = Direction;
        float spread = Mathf.Max(_spread * SpreadMultiplier.Result(), 0f);
        if (spread != 0)
        {
            Vector3 perp = GlobalBasis.X;
            float theta = (float)(_random.NextDouble() * 2.0 * Mathf.Pi);
            Vector3 rotationAxis = perp.Rotated(direction.Normalized(), theta).Normalized();

            float spreadAngle = Mathf.DegToRad((float)_random.NextDouble()*spread);
            direction = direction.Rotated(rotationAxis, spreadAngle);
        }

        ShootWithSpread(direction);
    }

    protected abstract void ShootWithSpread(Vector3 direction);

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