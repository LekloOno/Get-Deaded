using System;
using Godot;

[GlobalClass]
public partial class GC_HealthManager : Node
{
    [Export] public GC_Health TopHealthLayer {get; private set;}
    public HealthInitEventArgs InitState {get; private set;} = null;

    public EventHandler<HealthInitEventArgs> OnLayerInit;

    public bool Damage(float damage, out float takenDamage) => TopHealthLayer.TakeDamage(damage, out takenDamage);
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
}