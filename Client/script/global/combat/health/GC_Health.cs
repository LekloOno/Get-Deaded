using System;
using Godot;

public delegate void HealthEventHandler<T>(GC_Health senderLayer, T e);
public delegate void HealthEventHandler(GC_Health senderLayer);

[GlobalClass]
public partial class GC_Health : Node
{
    public GC_Health() {}
    public GC_Health(float maxHealth, GC_Health child) : this()
    {
        _maxHealth = maxHealth;
        Child = child;
        CurrentHealth = 0f;
    }

    [Export] protected float _maxHealth;

    private GC_Health _child;
    [Export] public GC_Health Child
    {
        get => _child;
        private set
        {
            if (value == _child)
                return;

            if (_child != null)
            {
                Child.OnDamage -= PropagDamage;
                Child.OnHeal -= PropagHeal;
                Child.OnBreak -= PropagBreak;
                Child.OnFull -= PropagFull;
                Child.OnDie -= PropagDie;
            }

            _child = value;

            if (Child != null)
            {
                Child.OnDamage += PropagDamage;
                Child.OnHeal += PropagHeal;
                Child.OnBreak += PropagBreak;
                Child.OnFull += PropagFull;
                Child.OnDie += PropagDie;  
            }
        }
    }
    public float CurrentHealth {get; protected set;}

    public HealthEventHandler<DamageEventArgs> OnDamage;
    private void PropagDamage(GC_Health sender, DamageEventArgs damage) =>
		OnDamage?.Invoke(sender, damage.Stack(CurrentHealth));

    public HealthEventHandler<DamageEventArgs> OnHeal;
    private void PropagHeal(GC_Health sender, DamageEventArgs heal) =>
		OnHeal?.Invoke(sender, heal.Stack(CurrentHealth));

    public HealthEventHandler<GC_Health> OnBreak; // Passes the child layer of the broken one as event arg
    private void PropagBreak(GC_Health sender, GC_Health childLayer) =>
		OnBreak?.Invoke(sender, childLayer);

    public HealthEventHandler<GC_Health> OnFull;  // Passes the parent layer of the full one as event arg
    private void PropagFull(GC_Health sender, GC_Health parentLayer) =>
		OnFull?.Invoke(sender, parentLayer);

    public HealthEventHandler OnDie;
    private void PropagDie(GC_Health sender) =>
        OnDie?.Invoke(sender);

    public virtual void Disable()
    {
        SpecDisable();
        Child?.Disable();
    }
    public virtual void SpecDisable() {} 

    public virtual void Enable()
    {
        SpecEnable();
        Child?.Enable();
    }
    public virtual void SpecEnable() {} 

    public void Initialize(out float totalInit, out float lowerInit, out float totalMax, out float lowerMax)
    {
        Enable();
        CurrentHealth = _maxHealth;

        if (Child != null)
        {
            Child.Initialize(out totalInit, out lowerInit, out totalMax, out lowerMax);
            totalInit += _maxHealth;
            totalMax += _maxHealth;
        }
        else
        {
            lowerInit = totalInit = lowerMax = totalMax = _maxHealth;
        }
    }

    protected DamageEventArgs DamageArgs(float amount) => new(amount, this);

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
    public virtual bool TakeDamage(float damage, out float takenDamage, out float overflow, out GC_Health deepest)
    {
        float damageTaken = ModifiedDamage(damage, out float remainingDamage);
        overflow = 0;
        CurrentHealth -= damageTaken;

        if (damageTaken > 0)
            OnDamage?.Invoke(this, DamageArgs(damageTaken));

        takenDamage = damageTaken;

        if (CurrentHealth > 0)
        {
            deepest = this;
            return false;
        }

        bool died = Propagate(remainingDamage, out float childDamage, out overflow, out deepest);
        takenDamage += childDamage;
        //OnDamage?.Invoke(this, DamageArgs(overflow));

        if (died)
            return true;
        
        if (damageTaken > 0)
            OnBreak?.Invoke(this, Child);
            
        return false;
    }

    private bool Propagate(float remainingDamage, out float takenDamage, out float overflow, out GC_Health deepest)
    {
        if (Child == null)
        {
            takenDamage = 0;
            overflow = remainingDamage;
            deepest = this;
            OnDie?.Invoke(this);
            return true;
        }

        return Child.TakeDamage(remainingDamage, out takenDamage, out overflow, out deepest);
    }

    public virtual float Heal(float healing, GC_Health parent)
    {
        float heal = (Child == null) ? healing : Child.Heal(healing, this);
        CurrentHealth += heal;
        OnHeal?.Invoke(this, DamageArgs(heal));
        
        if (CurrentHealth > _maxHealth)
        {
            float remainingHeal = CurrentHealth - _maxHealth;
            CurrentHealth = _maxHealth;
            OnFull?.Invoke(this, parent);

            return remainingHeal;
        }

        return 0;
    }

    public float TotalCurrent(out float lowerCurrent)
    {
        if (Child == null)
        {
            lowerCurrent = CurrentHealth;
            return CurrentHealth;
        }

        return CurrentHealth + Child.TotalCurrent(out lowerCurrent);
    }

    public bool IsLowerLayer() => Child == null;

    public GC_Health GetLowerLayer()
    {
        if (Child == null)
            return this;
        return Child.GetLowerLayer();
    }

    public GC_Health GetExposedLayer()
    {
        if (CurrentHealth > 0f || Child == null)
            return this;

        return Child.GetExposedLayer();
    }

/*
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

    

    public GC_Health GetLowerLayer()
    {
        if (Child == null)
            return this;
        return Child.GetLowerLayer();
    }*/
}