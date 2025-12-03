using System;
using Godot;

[GlobalClass]
public partial class PWF_FullAuto : PW_Fire
{
    private SceneTreeTimer _timer;
    public Action Stopped;
    public override void Disable() => StopShoot();

    protected override void SpecInitialize(PC_Shakeable shakeableCamera, PC_Recoil recoilController, GB_ExternalBodyManagerWrapper ownerBody){}


    protected override bool SpecPress()
    {
        StopShoot();
        _recoil.ResetBuffer();

        if (!CanShoot())
            return false;
        
        Shoot();
        _recoil?.Start();

        _timer = GetTree().CreateTimer(_fireRate/1000f);
        _timer.Timeout += ReShoot;
        return true;
    }
    protected override bool SpecRelease()
    {
        StopShoot();
        return true;
    }

    private void ReShoot()
    {
        if (!InfiniteMagazine && !_ammos.DidConsume(_ammosPerShot))
        {
            StopShoot();
            return;
        }

        Shoot();
        _recoil?.Add();
        _timer = GetTree().CreateTimer(_fireRate/1000f);
        _timer.Timeout += ReShoot;
    }

    private void StopShoot()
    {
        if (_timer != null)
        {
            _timer.Timeout -= ReShoot;
            _timer = null;
            _recoil?.Reset();
            Stopped?.Invoke();
        }
    }
}