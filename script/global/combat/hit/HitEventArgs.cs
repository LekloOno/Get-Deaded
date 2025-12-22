using System;

public class HitEventArgs : EventArgs
{
    [Flags]
    public enum HitFlags : byte
    {
        None = 0,
        Killed = 1 << 0,          // 0x1
        Missed = 1 << 1,          // 0x2
        Environment = 1 << 2,     // 0x4
        OverrideBodyPart = 1 << 7 // 0x80
    }

    /// <summary>
    /// The health manager of the entity it did hit.
    /// </summary>
    public GC_HealthManager Target {get;}
    /// <summary>
    /// The deepest health layer this hit did hit. <br/>
    /// For example, if a target with 40 shield points, and 20 health points behind this shield, receives 50 damage, then the deepest layer to be hit is the health layer.
    /// The shield receives 40 damages and propagates 10 damages to the health.
    /// </summary>
    public GC_Health SenderLayer {get;}
    /// <summary>
    /// The specific HurtBox this hit did hit.
    /// </summary>
    public GC_HurtBox HurtBox {get;}
    /// <summary>
    /// The amount of damage handled by the hit target.
    /// </summary>
    public float Damage {get;}
    /// <summary>
    /// The amount of damage that couldn't be handled by the hit target. <br />
    /// Exemple - target has 20 hps, and receives 30 damage. Overflow = 10.
    /// </summary>
    public float Overflow {get;}
    
    /// <summary>
    /// The specific hit object, like a projectile, scan data, etc.
    /// </summary>
    public GC_IHitDealer Dealer {get;}

    /// <summary>
    /// The entity responsible for this hit.
    /// </summary>
    public GE_IActiveCombatEntity Author {get;}
    /// <summary>
    /// Various flags about the hit. Below is a list of the correspondance to each bit. <br />
    ///  0 - 0x1    : This hit did kill. <br />
    ///  1 - 0x2    : This hit is a miss - it did hit a wall, ground, etc. <br />
    ///  2 - 0x4    : This hit did hit a damageable environment object. <br />
    ///  3 - ---    : <br />
    ///  <br />
    ///  4 - ---    : <br />
    ///  5 - ---    : <br />
    ///  6 - ---    : <br />
    ///  7 - 0x80 : Should be considered as a normal body hit by UI, statistics, etc.
    /// </summary>
    public HitFlags Flags { get; }
    /// <summary>
    /// Indicates the size of the hit. <br/>
    /// SubHit are used to emulate continuous fire at firerate that are too fast for engine ticks to keep up. </br>
    /// 1f is the default and represent a non-subed hit.
    /// </summary>
    public float SubHitSize { get; }

    public HitEventArgs(
        GC_HealthManager target,
        GC_Health senderLayer,
        GC_HurtBox hurtBox,
        float damage,
        bool kill,
        GC_IHitDealer dealer,
        GE_IActiveCombatEntity author,
        float overflow = 0,
        bool overrideBodyPart = false,
        bool env = false,
        float subHitSize = 1f
    ) : this(
        target, senderLayer, hurtBox, damage, dealer, author, overflow,
        (kill ? HitFlags.Killed : 0) |
        (overrideBodyPart ? HitFlags.OverrideBodyPart : 0) |
        (env ? HitFlags.Environment : 0),
        subHitSize){}

    public HitEventArgs(
        GC_HealthManager target,
        GC_Health senderLayer,
        GC_HurtBox hurtBox,
        float damage,
        GC_IHitDealer dealer,
        GE_IActiveCombatEntity author,
        float overflow,
        HitFlags flags,
        float subHitSize = 1f
    )
    {
        Target = target;
        SenderLayer = senderLayer;
        HurtBox = hurtBox;
        Damage = damage;
        Dealer = dealer;
        Author = author;
        Overflow = overflow;
        Flags = flags;
        SubHitSize = subHitSize;
    }

    public static HitEventArgs Miss(GC_IHitDealer dealer, GE_IActiveCombatEntity author) =>
        new(null, null, null, 0f, dealer, author, 0f, HitFlags.Missed);

    // Maybe replace ENV by a specific type of GC_HealthManager so we can still follow the statistics on environment
    public static HitEventArgs Env(GC_IHitDealer dealer, GE_IActiveCombatEntity author) =>
        new(null, null, null, 0f, dealer, author, 0f, HitFlags.Environment);

    /// <summary>
    /// Mostly for statistics & UI - if true, consider this hit as a normal body hit.
    /// </summary>
    public bool OverrideBodyPart => Flags.HasFlag(HitFlags.OverrideBodyPart);
    /// <summary>
    /// True if the hit did kill.
    /// </summary>
    public bool Killed => Flags.HasFlag(HitFlags.Killed);
    /// <summary>
    /// True if the hit did hit a damageable environment object.
    /// </summary>
    public bool IsEnv => Flags.HasFlag(HitFlags.Environment);
    /// <summary>
    /// True if the hit is a miss - it did hit a wall, ground, etc.
    /// </summary>
    public bool Missed => Flags.HasFlag(HitFlags.Missed);
    /// <summary>
    /// True if the hit did not miss - it did hit a damageable thing.
    /// </summary>
    public bool Hit => !Missed;
    /// <summary>
    /// Total damage delivered to the target. The sum of handled and overflowing damages.
    /// </summary>
    public float TotalDamage => Damage + Overflow;
}