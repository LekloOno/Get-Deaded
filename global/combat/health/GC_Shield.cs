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
        float realHeal = Mathf.Min(amount, _maxHealth - CurrentHealth);
        CurrentHealth += realHeal;

        OnHeal?.Invoke(this, DamageArgs(realHeal));

        if (CurrentHealth == _maxHealth)
            OnFull?.Invoke(this, null);
    }

    public void Decay(float amount)
    {
        float realDamage = Mathf.Min(amount, CurrentHealth);
        CurrentHealth -= realDamage;

        GD.Print(CurrentHealth);

        OnDamage?.Invoke(this, DamageArgs(realDamage));
        if (CurrentHealth == 0f)
            OnBreak?.Invoke(this, Child);
    }
}