namespace Pew;

public interface GC_IHitDealer
{
    GC_Hit HitData {get;}
    GE_IActiveCombatEntity OwnerEntity {get;}
    void Shoot();
}