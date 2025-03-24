using System;

public class ShotHitEventArgs : EventArgs
{
    public static readonly ShotHitEventArgs MISS = new(null, null, null, 0f, false);
    public GC_HealthManager Target {get;}
    public GC_Health SenderLayer {get;}
    public GC_HurtBox HurtBox {get;}
    public float Damage {get;}
    public bool Kill {get;}
    public ShotHitEventArgs(GC_HealthManager target, GC_Health senderLayer, GC_HurtBox hurtBox, float damage, bool kill)
    {
        SenderLayer = senderLayer;
        Target = target;
        HurtBox = hurtBox;
        Damage = damage;
        Kill = kill;    
    }
}