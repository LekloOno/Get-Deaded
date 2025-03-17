using System;

public class ShotHitEventArgs : EventArgs
{
    public GC_HealthManager Target {get ;}
    public GC_Hitbox Hitbox {get ;}
    public float Damage {get;}
    public bool Kill {get;}
    public ShotHitEventArgs(GC_HealthManager target, GC_Hitbox hitbox, float damage, bool kill)
    {
        Target = target;
        Hitbox = hitbox;
        Damage = damage;
        Kill = kill;    
    }
}