using System;
using Godot;

[GlobalClass]
public partial class GC_Health : Resource
{
    [Export] protected float _maxHealth;
    [Export] public GC_Health Child {get; private set;}
    public float CurrentHealth {get; protected set;}
    public delegate void HealthEventHandler<T>(GC_Health senderLayer, T e);
    public delegate void HealthEventHandler(GC_Health senderLayer);

    public HealthEventHandler<float> OnDamage;
    public HealthEventHandler<float> OnHeal;
    public HealthEventHandler<GC_Health> OnBreak; // Passes the child layer of the broken one as event arg
    public HealthEventHandler<GC_Health> OnFull;  // Passes the parent layer of the full one as event arg
    public HealthEventHandler OnDie;

    public void Initialize()
    {
        CurrentHealth = _maxHealth;

        if (Child != null)
        {
            Child.OnDamage += (o, damage) => OnDamage?.Invoke(o, damage);
            Child.OnHeal += (o, heal) => OnHeal?.Invoke(o, heal);
            Child.OnBreak += (o, childLayer) => OnBreak?.Invoke(o, childLayer);
            Child.OnFull += (o, parentLayer) => OnBreak?.Invoke(o, parentLayer);
            Child.OnDie += (o) => OnDie?.Invoke(o);

            Child.Initialize();
        }
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
        OnBreak?.Invoke(this, Child);
        return false;
    }

    private bool Propagate(float remaingDamage)
    {
        if (Child == null)
        {
            OnDie?.Invoke(this);
            return true;
        }

        return Child.TakeDamage(remaingDamage);
    }

    public virtual float Heal(float healing, GC_Health parent)
    {
        float heal = Child.Heal(healing, this);
        CurrentHealth += heal;
        OnHeal?.Invoke(this, heal);
        
        if (CurrentHealth > _maxHealth)
        {
            float remainingHeal = CurrentHealth - _maxHealth;
            CurrentHealth = _maxHealth;
            OnFull?.Invoke(this, parent);

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

    public GC_Health Exposed()
    {
        if (CurrentHealth > 0f)
            return this;

        return Child.Exposed();
    }

    public GC_Health GetLowerLayer()
    {
        if (Child == null)
            return this;
        return Child.GetLowerLayer();
    }

    public bool IsLowerLayer() => Child == null;
}