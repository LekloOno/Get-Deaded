using System;
using Godot;

[GlobalClass]
public partial class PWF_FullAutoBis : PW_FireBis
{
    private SceneTreeTimer _timer;
    public Action Stopped;
    private ulong _lastExpectedTime;
    protected override void DisableSpec() => StopShoot();

    protected override void SpecInitialize(GE_IActiveCombatEntity owner){}


    protected override bool SpecPress()
    {
        StopShoot();

        if (!CanShoot())
            return false;
        
        _lastExpectedTime = _fireRate;
        Shoot();

        _timer = GetTree().CreateTimer(_fireRate/1000f, false, true);
        _timer.Timeout += ReShoot;
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
        

    private void ReShoot()
    {
        if (!CanReShoot())
        {
            StopShoot();
            return;
        }

        ulong exactElapsedTime = PHX_Time.ScaledTicksMsec - _lastShot;

        Shoot();

        long timeUnaccuracy = (long) (exactElapsedTime - _lastExpectedTime);
        _lastExpectedTime = _fireRate - (ulong) timeUnaccuracy;
        
        _timer = GetTree().CreateTimer(_lastExpectedTime/1000f, false, true);
        _timer.Timeout += ReShoot;
    }

    private void StopShoot()
    {
        if (_timer != null)
        {
            _timer.Timeout -= ReShoot;
            _timer = null;
            Stopped?.Invoke();
        }
    }
}