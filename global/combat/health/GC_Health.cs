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

    protected virtual float ReductionFromDamage(float damage) => 0f;        // How much reduction for `damage` damage
    protected virtual float DamageFromReduction(float reduction) => 0f;     // How much damage were handled if the reduction was `reduction`
    
    /// <summary>
    /// Allows to set custom Overflow Behaviors.
    /// This base method only handles the damage its remaining health allows to handle.
    /// The overflowing damage will not be affected by this layer's modification.
    /// Example -
    ///     An armor with 50% resistance and 20 hps and child layer.
    ///     The entity receives 80 damages. Should we - 
    ///         - Handle 20*2 (20 / 50%) damage thus leaving the remaining 40 damages to the next layer ?
    ///         - Reduce the 80 damages to 40 and leave the remaining 20 damages to the next layer ?
    ///         - Don't even overflow, handle the whole damages ?
    ///     
    ///     The base default behavior described below corresponds to the first case.
    /// </summary>
    /// <param name="damage"></param>
    /// <returns>How much damage this layer will handle at its current state before overflowing onto its child.</returns>
    protected virtual float HandledDamage(float damage)
    {
        float effectiveReduction = ReductionFromDamage(CurrentHealth);
        return CurrentHealth + DamageFromReduction(effectiveReduction);
    }

    private float ModifiedDamage(float damage, out float remainingDamage)
    {
        float reduction = ReductionFromDamage(damage);
        float reducedDamage = damage - reduction;

        if (CurrentHealth >= reducedDamage)
            remainingDamage = 0f;
        else
        {
            remainingDamage = damage - HandledDamage(damage);
            reducedDamage = CurrentHealth;
        }

        return reducedDamage;
    }
    public virtual bool TakeDamage(float damage)
    {
        float damageTaken = ModifiedDamage(damage, out float remainingDamage);
        CurrentHealth -= damageTaken;

        GD.Print(CurrentHealth);

        OnDamage?.Invoke(this, damageTaken);

        if (CurrentHealth > 0)
            return false;

        if (Propagate(remainingDamage))
            return true;
        
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