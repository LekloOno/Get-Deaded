using Godot;

public interface GC_IHitDealer
{
    GC_Hit HitData {get;}
    void Shoot(Vector3 origin, Vector3 direction);
    //bool SendHit(GC_HurtBox hurtBox, out float takenDamage) => hurtBox.Damage(this, out takenDamage);
}