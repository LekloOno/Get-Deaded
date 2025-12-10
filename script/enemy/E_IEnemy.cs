using System;

public delegate void EnemyHealthEventHandler<T>(E_IEnemy enemy, GC_Health senderLayer, T e);
public delegate void EnemyHealthEventHandler(E_IEnemy enemy, GC_Health senderLayer);
public delegate void EnemyDisableEventHandler(E_IEnemy enemy);

public interface E_IEnemy : GE_CombatEntity
{
    public uint Score {get;}
    public EnemyHealthEventHandler OnDie {get; set;}
    public EnemyDisableEventHandler OnDisable {get; set;} // After death, allow some delay to do animations or else.
    public EnemyHealthEventHandler<DamageEventArgs> OnDamage {get; set;}

    /// <summary>
    /// A procedure to disable the enemy, for example, to use it in a pooling system. <br/>
    /// Typically disabling physics process.
    /// </summary>
    public void Disable();
    /// <summary>
    /// A procedure to enable a previously disabled enemy.
    /// Typically enabling physics process.
    /// </summary>
    public void Enable();
}