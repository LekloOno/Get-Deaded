using System;
using Godot;

public delegate void EnemyHealthEventHandler<T>(E_IEnemy enemy, GC_Health senderLayer, T e);
public delegate void EnemyHealthEventHandler(E_IEnemy enemy, GC_Health senderLayer);
public delegate void EnemyDisableEventHandler(E_IEnemy enemy);

public interface E_IEnemy : GE_IActiveCombatEntity, PHX_ListenPoolObject<E_IEnemy>
{
    public uint Score {get;}
    public event EnemyHealthEventHandler? Died;
    public event EnemyDisableEventHandler? Disabled; // After death, allow some delay to do animations or else.
    public event EnemyHealthEventHandler<DamageEventArgs>? Damaged;
    public GE_ICombatEntity Target {get; set;}
}