using System;

public class ShotHitEventArgs : EventArgs
{
    public static readonly ShotHitEventArgs MISS = new(null, null, 0f, false);
    public GC_HealthManager Target {get ;}
    public GC_HurtBox HurtBox {get ;}
    public float Damage {get;}
    public bool Kill {get;}
    public ShotHitEventArgs(GC_HealthManager target, GC_HurtBox hurtBox, float damage, bool kill)
    {
        Target = target;
        HurtBox = hurtBox;
        Damage = damage;
        Kill = kill;    
    }
}