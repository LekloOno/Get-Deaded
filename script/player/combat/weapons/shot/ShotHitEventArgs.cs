using System;

public class ShotHitEventArgs : EventArgs
{
    public GC_HealthManager Target {get;}
    public GC_Health SenderLayer {get;}
    public GC_HurtBox HurtBox {get;}
    public float Damage {get;}
    /// <summary>
    /// Overflowing damage. That is, the amount of damage that couldn't be handled by the target. <br />
    /// Exemple - target has 20 hps, and receives 30 damage. Overflow = 10.
    /// </summary>
    public float Overflow {get;}
    public PW_Weapon Weapon {get;}
    public PW_Fire Fire {get;}
    public GE_CombatEntity Author {get;}
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
    public byte Flags {get;}

    public ShotHitEventArgs(
        GC_HealthManager target,
        GC_Health senderLayer,
        GC_HurtBox hurtBox,
        float damage,
        bool kill,
        PW_Weapon weapon,
        PW_Fire fire,
        GE_CombatEntity author,
        float overflow = 0,
        bool overrideBodyPart = false,
        bool env = false
    ) : this(
        target, senderLayer, hurtBox, damage, weapon, fire, author, overflow,
        (byte)((kill ? 1 : 0) | (overrideBodyPart ? 1 : 0) << 7 | (env ? 1 : 0) << 2)){}

    public ShotHitEventArgs(
        GC_HealthManager target,
        GC_Health senderLayer,
        GC_HurtBox hurtBox,
        float damage,
        PW_Weapon weapon,
        PW_Fire fire,
        GE_CombatEntity author,
        float overflow,
        byte flags
    )
    {
        Target = target;
        SenderLayer = senderLayer;
        HurtBox = hurtBox;
        Damage = damage;
        Weapon = weapon;
        Fire = fire;
        Author = author;
        Overflow = overflow;
        Flags = flags;
    }

    public static ShotHitEventArgs Miss(PW_Weapon weapon, PW_Fire fire, GE_CombatEntity author) =>
        new(null, null, null, 0f, weapon, fire, author, 0f, 0x2);

    // Maybe replace ENV by a specific type of GC_HealthManager so we can still follow the statistics on environment
    public static ShotHitEventArgs Env(PW_Weapon weapon, PW_Fire fire, GE_CombatEntity author) =>
        new(null, null, null, 0f, weapon, fire, author, 0f, 0x4);

    /// <summary>
    /// Mostly for statistics & UI - if true, consider this hit as a normal body hit.
    /// </summary>
    public bool OverrideBodyPart => (Flags & 0x80) != 0;
    /// <summary>
    /// True if the hit did kill.
    /// </summary>
    public bool Killed => (Flags & 0x1) != 0;
    /// <summary>
    /// True if the hit did hit a damageable environment object.
    /// </summary>
    public bool IsEnv => (Flags & 0x4) != 0;
    /// <summary>
    /// True if the hit is a miss - it did hit a wall, ground, etc.
    /// </summary>
    public bool Missed => (Flags & 0x2) != 0;
    /// <summary>
    /// True if the hit did not miss - it did hit a damageable thing.
    /// </summary>
    public bool Hit => (Flags & 0x2) == 0;

    public float TotalDamage() => Damage + Overflow;
}