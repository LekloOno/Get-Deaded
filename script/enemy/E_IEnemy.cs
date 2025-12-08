public delegate void EnemyHealthEventHandler<T>(E_IEnemy enemy, GC_Health senderLayer, T e);
public delegate void EnemyHealthEventHandler(E_IEnemy enemy, GC_Health senderLayer);

public interface E_IEnemy : GE_CombatEntity
{
    public uint Score {get;}
    public EnemyHealthEventHandler OnDie {get; set;}
    public EnemyHealthEventHandler<DamageEventArgs> OnDamage {get; set;}
}