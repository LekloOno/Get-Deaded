using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public partial class GC_Shield : GC_Health
{
    [Export] private float _regenPerSec;
    [Export] private double _regenDelay;
    private ulong _lastActivate;
    private bool _active;
    private Timer _regenTimer;

    public override float Heal(float healing, GC_Health parent) => Child.Heal(healing, this);

    public override void _Ready()
    {
        _regenTimer = new();
        AddChild(_regenTimer);
        _regenTimer.Timeout += StartRegen;
        OnDamage += DamageStartTimer;
        SetPhysicsProcess(false);
    }

    private void DamageStartTimer(GC_Health __, DamageEventArgs ___)
    {
        StopRegen();
        _regenTimer.Start(_regenDelay);
    }

    public void StartRegen()
    {
        SetPhysicsProcess(true);
    }

    public void StopRegen()
    {
        SetPhysicsProcess(false);
    }

    public override void _PhysicsProcess(double delta)
    {
        Regen(_regenPerSec* (float) delta);
    }

    public void Regen(float amount)
    {
        float realHeal = Mathf.Min(amount, _maxHealth - CurrentHealth);
        CurrentHealth += realHeal;

        OnHeal?.Invoke(this, DamageArgs(realHeal));

        if (CurrentHealth != _maxHealth)
            return;
        
        OnFull?.Invoke(this, null);
        StopRegen();
    }
}