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
    [Export] protected float _spread = 0f;          // In degrees
    [Export] protected float _knockBack = 0f;
    [Export] protected bool _ignoreCrit = false;      // For visual and sound mostly
    [Export] private float _kickBack = 0f;

    [ExportCategory("Visuals")]
    [Export] protected PW_Trail _trail;
    [Export] private PCT_UndirectScalable _traumaCauser;
    [Export] private bool _clampTrauma = true;
    [Export] private float _maxTrauma = 0.2f;
    [Export] private float _ragdollFactor = 1f;

    private GB_ExternalBodyManagerWrapper _ownerBody;
    public PW_Fire Fire {get; protected set;}
    protected PW_Weapon _weapon => Fire.Weapon;
    public GE_IActiveCombatEntity OwnerEntity => Fire.Weapon.Handler.OwnerEntity;

    public EventHandler<HitEventArgs> Hit;
    public MATH_AdditiveModifiers SpreadMultiplier {get; private set;} = new();
    public float Spread => _spread * SpreadMultiplier.Result();
    public MATH_AdditiveModifiers KnockBackMultiplier {get; private set;} = new();
    public MATH_FlatVec3Modifiers KnockBackDirFlatAdd {get; private set;} = new();
    public float KnockBack => _knockBack * KnockBackMultiplier.Result();
    public MATH_AdditiveModifiers DamageMultipler => _hitData.DamageMultiplier;
    public GC_Hit HitData => _hitData;
    public Vector3 Direction => -GlobalBasis.Z; 
    private static Random _random = new();

    public void Initialize(GB_ExternalBodyManagerWrapper ownerBody, PW_Fire fire)
    {
        Fire = fire;
        _ownerBody = ownerBody;
        _hitData.InitializeModifiers();
        SpecInitialize(ownerBody);
    }

    public abstract void SpecInitialize(GB_ExternalBodyManagerWrapper ownerBody);

    public void Shoot()
    {
        Vector3 direction = Direction;
        float spread = Mathf.Max(Spread, 0f);
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

    protected void HandleKick(Vector3 direction)
    {
        if (_kickBack != 0)
            _ownerBody.HandleKnockBack(-direction.Normalized() * _kickBack);
    }

    /// <summary>
    /// Handles redundant operation to send a hit to a target Hurtbox. <br/>
    /// <br/>
    /// `globalKbDir` is optional and overrides the direction used to apply global knockback. If unspecified, it will use the normalized vector between the origin (from) and hit position. 
    /// </summary>
    /// <param name="hurtBox"></param>
    /// <param name="hitPosition"></param>
    /// <param name="from"></param>
    /// <param name="globalKbDir">optional - override the direction used to apply global knockback. If unspecified, it will use the normalized vector between the origin (from) and hit position. </param>
    /// <returns></returns>
    protected HitEventArgs SendHit(GC_HurtBox hurtBox, Vector3 hitPosition, Vector3 from, Vector3? globalKbDir = null)
    {
        Vector3 direction = (hitPosition - from).Normalized();

        Vector3? globalKnockBack = null;

        if (globalKbDir is Vector3 overrideDirection)
            globalKnockBack = NullableKnockBack(overrideDirection);
        else
            globalKnockBack = NullableKnockBack(direction);
        
        Vector3 localKnockBack = direction * _hitData.Damage * _ragdollFactor;

        HitEventArgs reg = hurtBox.HandleHit(
            OwnerEntity, this, hitPosition, from,
            localKnockBack, globalKnockBack,
            _ignoreCrit
        );

        DoHit(reg, hitPosition);
        return reg;
    }

    protected void DoHit(HitEventArgs e, Vector3 hitPosition)
    {
        Hit?.Invoke(this, e);
        
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

    protected Vector3 KnockBackFrom(Vector3 direction) =>
        KnockBack * direction.Normalized() + KnockBackDirFlatAdd.Result();

    private Vector3? NullableKnockBack(Vector3 direction)
    {
        if (_knockBack == 0f)
            return null;

        Vector3 impulse = KnockBackFrom(direction);
        if (impulse.Length() == 0f)
            return null;

        return impulse;
    }
}