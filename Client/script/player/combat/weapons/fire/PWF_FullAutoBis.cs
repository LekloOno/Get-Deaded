using System;
using Godot;

[GlobalClass]
public partial class PWF_FullAutoBis : PW_FireBis
{
    public event Action? Stopped;
    protected override void DisableSpec() => StopShoot();
    protected override void SpecInitialize(GE_IActiveCombatEntity owner) {}

    public override void _Ready()
    {
        base._Ready();
        SetPhysicsProcess(false);
    }

    protected override bool SpecPress()
    {
        StopShoot();

        if (!CanShoot())
            return false;
        
        Shoot();

        _autoAccumulator = 0;
        SetPhysicsProcess(true);
        return true;
    }
    protected override bool SpecRelease()
    {
        StopShoot();
        return true;
    }

    private bool CanReShoot() =>
        _enabled &&
        (InfiniteMagazine || _ammos.DidConsume(_ammosPerShot));

    private double _autoAccumulator = 0;
    public override void _PhysicsProcess(double delta)
    {
        _autoAccumulator += delta;

        double fireRateSec = _fireRate / 1000.0;
        
        while (_autoAccumulator >= fireRateSec)
        {
            _autoAccumulator -= fireRateSec;

            if (!CanReShoot())
            {
                StopShoot();
                return;
            }

            Shoot();
        }
    }

    private void StopShoot()
    {
        if (!IsPhysicsProcessing())
            return;

        SetPhysicsProcess(false);

        _autoAccumulator = 0;
        Stopped?.Invoke();
    }
}