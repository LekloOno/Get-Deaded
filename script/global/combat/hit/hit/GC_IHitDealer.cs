using Godot;

public interface GC_IHitDealer
{
    GC_Hit HitData {get;}
    void Shoot();
    //bool SendHit(GC_HurtBox hurtBox, out float takenDamage) => hurtBox.Damage(this, out takenDamage);
}