using System;
using Godot;

[GlobalClass]
public partial class GC_HealthManager : Node
{
    [Export] public GC_Health TopHealthLayer {get; private set;}
    public HealthInitEventArgs InitState {get; private set;} = null;

    public EventHandler<HealthInitEventArgs> OnLayerInit;

    public bool Damage(float damage) => TopHealthLayer.TakeDamage(damage);
    public float Heal(float heal) => TopHealthLayer.Heal(heal, null);

    public override void _Ready()
    {
        TopHealthLayer.Initialize(out float totalInit, out float lowerInit, out float totalMax, out float lowerMax);
        InitState = new(totalInit, lowerInit, totalMax, lowerMax);
        OnLayerInit?.Invoke(this, InitState);
    }
    public override void _PhysicsProcess(double delta)
    {
        // To implement
    }
    public override void _Process(double delta)
    {
        // To implement
    }
}