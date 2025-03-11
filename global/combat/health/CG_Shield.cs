using System;
using Godot;

[GlobalClass]
public partial class CG_Shield : GC_Health
{
    private bool _active;
    public CG_Shield(float maxHealth) : base(maxHealth) {}
    public CG_Shield(float maxHealth, GC_Health child) : base(maxHealth, child) {}
    public CG_Shield(float maxHealth, float initialHealth, GC_Health child) : base(maxHealth, initialHealth, child) {}

    public override bool TakeDamage(float damage)
    {
        if (_active)
            return base.TakeDamage(damage);

        return _child.TakeDamage(damage);
    }

    public override float Heal(float healing)
    {
        return _child.Heal(healing);
    }

    public void Restore(float amount)
    {
        _currentHealth += amount;

        if (_currentHealth > _maxHealth)
        {
            _currentHealth = 0f;
            OnFull?.Invoke(this, EventArgs.Empty);
        }
    }
}