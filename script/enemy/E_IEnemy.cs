using System;
using Godot;

namespace Pew;

public delegate void EnemyHealthEventHandler<T>(E_IEnemy enemy, GC_Health senderLayer, T e);
public delegate void EnemyHealthEventHandler(E_IEnemy enemy, GC_Health senderLayer);
public delegate void EnemyDisableEventHandler(E_IEnemy enemy);

public interface E_IEnemy : GE_IActiveCombatEntity, PHX_PoolObject
{
    public uint Score {get;}
    public EnemyHealthEventHandler OnDie {get; set;}
    public EnemyDisableEventHandler OnDisable {get; set;} // After death, allow some delay to do animations or else.
    public EnemyHealthEventHandler<DamageEventArgs> OnDamage {get; set;}
    public GE_ICombatEntity Target {get; set;}
}