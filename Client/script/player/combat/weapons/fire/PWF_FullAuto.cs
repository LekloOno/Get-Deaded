using System;
using Godot;

[GlobalClass]
public partial class PWF_FullAuto : PW_Fire
{
    public event Action? Stopped;
    public override void DisableSpec() => StopShoot();

    protected override void SpecInitialize(PC_Shakeable shakeableCamera, PC_Recoil recoilController, GB_ExternalBodyManagerWrapper ownerBody){}

    public override void _Ready()
    {
        base._Ready();
        SetPhysicsProcess(false);
    }

    protected override bool SpecPress()
    {
        StopShoot();
        _recoil.ResetBuffer();

        if (!CanShoot())
            return false;
        
        Shoot();
        _recoil?.Start();

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
            _recoil?.Add();
        }
    }

    private void StopShoot()
    {
        if (!IsPhysicsProcessing())
            return;

        SetPhysicsProcess(false);

        _autoAccumulator = 0;
        _recoil?.Reset();
        Stopped?.Invoke();
    }
}