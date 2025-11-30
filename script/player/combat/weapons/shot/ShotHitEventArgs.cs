using System;

public class ShotHitEventArgs : EventArgs
{
    public static readonly ShotHitEventArgs MISS = new(null, null, null, 0f, false);
    // Maybe replace ENV by a specific type of GC_HealthManager so we can still follow the statistics on environment
    public static readonly ShotHitEventArgs ENV = new(null, null, null, 0f, false);
    
    public GC_HealthManager Target {get;}
    public GC_Health SenderLayer {get;}
    public GC_HurtBox HurtBox {get;}
    public float Damage {get;}
    public bool Kill {get;}
    public float Overflow {get;}
    public bool OverrideBodyPart {get;}

    public ShotHitEventArgs(GC_HealthManager target, GC_Health senderLayer, GC_HurtBox hurtBox, float damage, bool kill, float overflow = 0, bool overrideBodyPart = false)
    {
        SenderLayer = senderLayer;
        Target = target;
        HurtBox = hurtBox;
        Damage = damage;
        Kill = kill;
        Overflow = overflow;
        OverrideBodyPart = overrideBodyPart;
    }

    public float TotalDamage() => Damage + Overflow;
    public bool IsNotEnemy() => this == MISS || this == ENV;
}