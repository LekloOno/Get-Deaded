using System;
using Godot;
using Godot.Collections;

namespace Pew;

[GlobalClass]
public partial class GC_HealthManager : Node3D
{
    [Export] public GB_ExternalBodyManagerWrapper _body;
    [Export] public GC_Health TopHealthLayer {get; set;}
    [Export] private Array<GC_HurtBox> _hurtBoxes;
    [Export] private CONF_HurtBoxFaction _hurtMask = CONF_HurtBoxFaction.Enemy;
    public HealthInitEventArgs InitState {get; private set;} = null;

    public EventHandler<HealthInitEventArgs> OnLayerInit;

    public virtual bool Damage(
        GC_IHitDealer hitDealer,
        float expectedDamage,
        out float takenDamage,
        out float overflow,
        out GC_Health deepest
    ) => TopHealthLayer.TakeDamage(expectedDamage, out takenDamage, out overflow, out deepest);

    public float Heal(float heal) => TopHealthLayer.Heal(heal, null);
    public override void _Ready()
    {
        TopHealthLayer.OnDie += Die;
        Init();
    }

    public void Die(GC_Health _)
    {
        DisableHurt();
        TopHealthLayer.Disable();
    }

    public void Init(bool reInit = false)
    {
        EnableHurt(_hurtMask.LayerMask());
        TopHealthLayer.Initialize(out float totalInit, out float lowerInit, out float totalMax, out float lowerMax, reInit);
        InitState = new(totalInit, lowerInit, totalMax, lowerMax, reInit);
        OnLayerInit?.Invoke(this, InitState);
    }
    public GC_Health GetLowerLayer() => TopHealthLayer.GetLowerLayer();
    public GC_Health GetExposedLayer() => TopHealthLayer.GetExposedLayer();

    public void EnableHurt(uint collisionLayer)
    {
        foreach (GC_HurtBox hurtBox in _hurtBoxes)
            hurtBox.CollisionLayer = collisionLayer;
    }

    public void DisableHurt()
    {
        foreach (GC_HurtBox hurtBox in _hurtBoxes)
            hurtBox.CollisionLayer = 0;
    }

    public void HandleKnockBack(Vector3 force)
    {
        _body.HandleKnockBack(force);
    }

    public HealthEventHandler OnDie
    {
        get => TopHealthLayer.OnDie;
        set => TopHealthLayer.OnDie = value;
    }
    public HealthEventHandler<DamageEventArgs> OnDamage
    {
        get => TopHealthLayer.OnDamage;
        set => TopHealthLayer.OnDamage = value;
    }
    public HealthEventHandler<DamageEventArgs> OnHeal
    {
        get => TopHealthLayer.OnHeal;
        set => TopHealthLayer.OnHeal = value;
    }

    public HealthEventHandler<GC_Health> OnBreak
    {
        get => TopHealthLayer.OnBreak;
        set => TopHealthLayer.OnBreak = value;
    }

    public HealthEventHandler<GC_Health> OnFull
    {
        get => TopHealthLayer.OnFull;
        set => TopHealthLayer.OnFull = value;
    }
}