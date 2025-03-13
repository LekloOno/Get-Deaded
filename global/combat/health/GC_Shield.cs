using System;
using Godot;

[GlobalClass]
public partial class GC_Shield : GC_Health
{
    public bool _active;
    public EventHandler OnActivate;
    public EventHandler OnDeactivate;

    public void Activate()
    {
        if (_active)
            return;
        
        _active = true;
        OnActivate?.Invoke(this, EventArgs.Empty);
    }

    public void Deactivate()
    {
        if (!_active)
            return;

        _active = false;
        OnDeactivate?.Invoke(this, EventArgs.Empty);
    }

    public override bool TakeDamage(float damage)
    {
        if (_active)
            return base.TakeDamage(damage);

        return Child.TakeDamage(damage);
    }

    public override float Heal(float healing, GC_Health parent) => Child.Heal(healing, this);

    public void Regen(float amount)
    {
        CurrentHealth += amount;

        OnHeal?.Invoke(this, amount);
        if (CurrentHealth > _maxHealth)
        {
            CurrentHealth = _maxHealth;
            OnFull?.Invoke(this, null);
        }
    }

    public void Decay(float amount)
    {
        CurrentHealth -= amount;

        OnDamage?.Invoke(this, amount);
        if (CurrentHealth <= 0f)
        {
            CurrentHealth = 0f;
            OnBreak?.Invoke(this, Child);
        }
    }
}