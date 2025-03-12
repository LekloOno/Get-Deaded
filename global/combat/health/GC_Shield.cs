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

    public override float Heal(float healing)
    {
        return Child.Heal(healing);
    }

    public void Regen(float amount)
    {
        CurrentHealth += amount;

        if (CurrentHealth > _maxHealth)
        {
            CurrentHealth = _maxHealth;
            OnFull?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Decay(float amount)
    {
        CurrentHealth -= amount;

        if (CurrentHealth <= 0f)
        {
            CurrentHealth = 0f;
            OnBreak?.Invoke(this, EventArgs.Empty);
        }
    }
}