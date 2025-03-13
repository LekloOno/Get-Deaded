using System;
using Godot;

[GlobalClass]
public partial class PH_Manager : GC_HealthManager
{
    [Export] private GC_Shield _shield;
    [Export] private float _decayPerSecond;
    [Export] private float _regenPerSecond;

    private EventHandler<double> OnProcess;

    public override void _Ready()
    {
        _shield.OnBreak += (o, e) => _shield.Deactivate();
        _shield.OnActivate += OnActivate;
        _shield.OnDeactivate += OnDeactivate;
        _shield.OnFull += (o, e) => OnProcess -= Regen;

        OnProcess += Regen;

        base._Ready();
    }
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("shield"))
            _shield.Activate();

        else if (@event.IsActionReleased("shield"))
            _shield.Deactivate();
    }
    public void OnActivate(object sender, EventArgs e)
    {
        OnProcess -= Regen;
        OnProcess += Decay;
    }

    public void OnDeactivate(object sender, EventArgs e)
    {
        OnProcess -= Decay;
        OnProcess += Regen;
    }

    public void Regen(object sender, double delta) => _shield.Regen((float)delta * _regenPerSecond);
    public void Decay(object sender, double delta) => _shield.Decay((float)delta * _decayPerSecond);

    public override void _PhysicsProcess(double delta) => OnProcess?.Invoke(this, delta);
}