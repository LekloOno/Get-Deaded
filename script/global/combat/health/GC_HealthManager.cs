using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class GC_HealthManager : Node3D
{
    [Export] public GB_ExternalBodyManager _body;
    [Export] public GC_Health TopHealthLayer {get; private set;}
    [Export] private Array<GC_HurtBox> _hurtBoxes;
    public HealthInitEventArgs InitState {get; private set;} = null;

    public EventHandler<HealthInitEventArgs> OnLayerInit;

    public virtual bool Damage(GC_IHitDealer hitDealer, float expectedDamage, out float takenDamage) => TopHealthLayer.TakeDamage(expectedDamage, out takenDamage);
    public float Heal(float heal) => TopHealthLayer.Heal(heal, null);
    public override void _Ready() => Init();

    public void Init(bool reInit = false)
    {
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
}