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
    [Export] protected bool _ignoreCrit = false;      // For visual and sound mostly
    [Export] private float _kickBack = 0f;

    [ExportCategory("Visuals")]
    [Export] protected PW_Trail _trail;
    [Export] private PCT_UndirectScalable _traumaCauser;
    [Export] private bool _clampTrauma = true;
    [Export] private float _maxTrauma = 0.2f;
    [Export] private float _ragdollFactor = 1f;
    private PW_IKnockBack _knockBack;
    public MATH_AdditiveModifiers KnockBackMultiplier => _knockBack?.KnockBackMultiplier;
    public MATH_FlatVec3Modifiers KnockBackDirFlatAdd => _knockBack?.KnockBackDirFlatAdd;

    private GB_ExternalBodyManagerWrapper _ownerBody;
    public PW_Fire Fire {get; protected set;}
    protected PW_Weapon _weapon => Fire.Weapon;
    public GE_IActiveCombatEntity OwnerEntity => Fire.Weapon.Handler.OwnerEntity;

    public EventHandler<HitEventArgs> Hit;
    public MATH_AdditiveModifiers SpreadMultiplier {get; private set;} = new();
    public float Spread => _spread * SpreadMultiplier.Result();
    public MATH_AdditiveModifiers KickBackMultiplier {get; private set;} = new();
    /// <summary>
    /// A damage Multiplier related to one individual shot. It is consumed for each fired shot.
    /// </summary>
    public MATH_AdditiveModifiers TempDamageMultiplier {get; private set;} = new();
    public MATH_AdditiveModifiers TempKnockBackMultiplier {get; private set;} = new();
    public MATH_AdditiveModifiers DamageMultiplier => _hitData.DamageMultiplier;
    public GC_Hit HitData => _hitData;
    public Vector3 Direction => -GlobalBasis.Z; 
    private static Random _random = new();
    public float KickBack => _kickBack * KickBackMultiplier.Result() * _partialMultiplier;
    public float Damage => _hitData.Damage * _partialMultiplier;
    public float HitTrauma => _maxTrauma * _partialMultiplier;

    private float _tempDamageMultiplier = 1f;
    private float _tempKnockBackMultiplier = 1f;


    private float _partialMultiplier = 1f;

    public override sealed void _Ready()
    {
        _knockBack = GetKnockBack();
        ReadySpec();
    }
    /// <summary>
    /// Allows the implementing class to define further custom _Ready routines. <br/>
    /// <br/>
    /// This prevents the user from unintendedly hiding important PW_Shot's _Ready base routines. <br/>
    /// It is called after the PW_Shot base routines.
    /// </summary>
    protected virtual void ReadySpec(){}

    private PW_IKnockBack GetKnockBack()
    {
        foreach (Node node in GetChildren())
            if (node is PW_IKnockBack knockBack)
                return knockBack;

        return null;
    }

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
        _tempDamageMultiplier = TempDamageMultiplier.Consume();
        _tempKnockBackMultiplier = TempKnockBackMultiplier.Consume();

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

    /// <summary>
    /// Allows to shoot shot fragments, when fire rate can't match physics tick rate. <br/>
    /// "size" is the size of the partial shot, like 0.5 if half a shot should be emulated. <br/>
    /// If greater than 1, then multiple independent shot will be emulated. <br/>
    /// <br/>
    /// You can redefine its behavior if required for your specific case. 
    /// </summary>
    /// <param name="size">The sub-shot amount. If greater than 1, it will shoot multiple shots.</param>
    public virtual void PartialShoot(double size)
    {
        while (size > 0)
        {
            _partialMultiplier = (float) Math.Min(size, 1);
            Shoot();
            size -= 1;
        }
        _partialMultiplier = 1f;
    }

    protected abstract void ShootWithSpread(Vector3 direction);

    protected void HandleKick(Vector3 direction)
    {
        float kickBack = KickBack;
        if (kickBack != 0)
            _ownerBody.HandleKnockBack(-direction.Normalized() * kickBack);
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

        Vector3? globalKnockBack;
        if (globalKbDir is Vector3 overrideDirection)
            globalKnockBack = _knockBack?.KnockBackImpulse(overrideDirection);
        else
            globalKnockBack = _knockBack?.KnockBackImpulse(direction);
        
        globalKnockBack *= _tempKnockBackMultiplier;
        // Do not use partial hit ragdoll here for now since we only trigger ragdoll on hit
        // But if we add active ragdoll later, it might be better to consider using scaled Damage.
        Vector3 localKnockBack = direction * Damage * _ragdollFactor;

        HitEventArgs reg = hurtBox.HandleHit(
            OwnerEntity, this, hitPosition, from,
            localKnockBack, globalKnockBack,
            _ignoreCrit, _tempDamageMultiplier, _partialMultiplier
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
            _traumaCauser.CauseClampedTrauma(HitTrauma);
        else
            _traumaCauser.CauseTrauma();
    }

    public virtual void Interrupt(){}
}