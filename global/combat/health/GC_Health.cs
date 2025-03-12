using System;
using Godot;

[GlobalClass]
public partial class GC_Health : Resource
{
    [Export] protected float _maxHealth;
    [Export] public GC_Health Child {get; private set;}
    public float CurrentHealth {get; protected set;}
    public EventHandler<float> OnDamage;
    public EventHandler<float> OnHeal;
    public EventHandler OnBreak;
    public EventHandler OnDie;
    public EventHandler OnFull;

    public GC_Health() : this(100, null) {}
    public GC_Health(float maxHealth, GC_Health child) : this(maxHealth, maxHealth, child) {}
    public GC_Health(float maxHealth, float initHealth, GC_Health child)
    {
        _maxHealth = maxHealth;
        CurrentHealth = initHealth;
        Child = child;
    }


    protected virtual float ModifiedDamage(float damage) => damage;
    public virtual bool TakeDamage(float damage)
    {
        float damageTaken = ModifiedDamage(damage);
        CurrentHealth -= damageTaken;
        GD.Print(CurrentHealth);
        OnDamage?.Invoke(this, damageTaken);

        if (CurrentHealth > 0)
            return false;

        if (Propagate(Mathf.Abs(CurrentHealth)))
            return true;
        
        CurrentHealth = 0;
        OnBreak?.Invoke(this, EventArgs.Empty);
        return false;
    }

    private bool Propagate(float remaingDamage)
    {
        if (Child == null)
        {
            OnDie?.Invoke(this, EventArgs.Empty);
            return true;
        }

        return Child.TakeDamage(remaingDamage);
    }

    public virtual float Heal(float healing)
    {
        float heal = Child.Heal(healing);
        CurrentHealth += heal;
        OnHeal?.Invoke(this, heal);
        
        if (CurrentHealth > _maxHealth)
        {
            float remainingHeal = CurrentHealth - _maxHealth;
            CurrentHealth = _maxHealth;
            OnFull?.Invoke(this, EventArgs.Empty);

            return remainingHeal;
        }

        return 0;
    }

    public float LowerMax()
    {
        if (Child == null)
            return _maxHealth;
        return Child.LowerMax();
    }

    public float LowerCurrent()
    {
        if (Child == null)
            return CurrentHealth;
        return Child.LowerCurrent();
    }

    public float HigherMax()
    {
        if (Child == null)
            return 0;
        return _maxHealth + Child.HigherMax();
    }

    public float HigherCurrent()
    {
        if (Child == null)
            return 0;
        return CurrentHealth + Child.HigherCurrent();
    }
}