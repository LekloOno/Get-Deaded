using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public partial class GC_Shield : GC_Health
{
    [Export] private ulong _parryTime;
    [Export] private ulong _absorbTime;
    private ulong _lastActivate;
    private bool _active;
    public EventHandler OnActivate;
    public EventHandler OnDeactivate;
    public HealthEventHandler<DamageEventArgs> OnDecay;
    public List<(ulong, float)> DamageBuffer = [];

    public void Activate()
    {
        if (_active)
            return;
        
        _lastActivate = Time.GetTicksMsec();
        _active = true;
        OnActivate?.Invoke(this, EventArgs.Empty);
    }

    public void Deactivate()
    {
        if (!_active)
            return;

        Absorb();
        _active = false;
        OnDeactivate?.Invoke(this, EventArgs.Empty);
    }

    public bool IsParrying() => _active && Time.GetTicksMsec() - _lastActivate < _parryTime;
    public void Absorb()
    {
        ulong later = Time.GetTicksMsec() - _absorbTime;
        float absorbed = DamageBuffer.Sum(t => t.Item1 > later ? t.Item2 : 0);
        Regen(absorbed);
        DamageBuffer.Clear();
    }

    public override bool TakeDamage(float damage, out float takenDamage, out float overflow, out GC_Health deepest)
    {
        if (_active)
        {
            DamageBuffer.Add((Time.GetTicksMsec(), damage));
            return base.TakeDamage(damage, out takenDamage, out overflow, out deepest);
        }

        return Child.TakeDamage(damage, out takenDamage, out overflow, out deepest);
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

        OnDecay?.Invoke(this, DamageArgs(realDamage));
        if (CurrentHealth == 0f)
            OnBreak?.Invoke(this, Child);
    }
}