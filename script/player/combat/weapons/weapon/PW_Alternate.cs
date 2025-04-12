using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class PW_Alternate : PW_Weapon
{
    [Export] private PW_Fire _primaryFire;
    [Export] private PW_Fire _secondaryFire;
    private PW_Fire _currentFire;


    protected override void SpecInitialize(PC_Shakeable shakeableCamera, PC_Recoil recoilController, GB_ExternalBodyManager owberBody)
    {
        _currentFire = _primaryFire;
        _primaryFire.Initialize(shakeableCamera, _camera, _sight, _barrel, recoilController, owberBody);
        _secondaryFire.Initialize(shakeableCamera, _camera, _sight, _barrel, recoilController, owberBody);
        _primaryFire.Hit += (o, e) => Hit?.Invoke(o, e);
        _secondaryFire.Hit += (o, e) => Hit?.Invoke(o, e);
    }

    protected override void SpecDisable() => _currentFire.Disable();
    protected override bool SpecPrimaryPress() => _currentFire.HandlePress();
    protected override bool SpecPrimaryRelease() => _currentFire.HandleRelease();
    protected override bool SpecSecondaryPress() => _secondaryFire.HandlePress();
    protected override bool SpecSecondaryRelease() => _secondaryFire.HandleRelease();


    protected override void SpecStartADS()
    {
        _currentFire.Disable();
        _currentFire = _secondaryFire;
    }

    protected override void SpecStopADS()
    {
        _currentFire.Disable();
        _currentFire = _primaryFire;
    }
    protected override void SpecReload()
    {
        _primaryFire.Reload();
        _secondaryFire.Reload();
    }
    protected override bool SpecCanReload(out bool tactical) => _primaryFire.CanReload(out tactical) || _secondaryFire.CanReload(out tactical);

    public override bool PickAmmo(int amount, bool magazine, int targetFireIndex)
    {
        if (targetFireIndex == 0)
        {
            int distribAmount = amount / 2;
            int remainder = amount % 2;
            bool primary = _primaryFire.PickAmmo(distribAmount + remainder, magazine);
            bool secondary = _secondaryFire.PickAmmo(distribAmount, magazine);
            return primary || secondary;
        }

        int realTargetIndex = targetFireIndex - 1;
        if ((realTargetIndex & 1) == 0)
            return _primaryFire.PickAmmo(amount, magazine);

        return _secondaryFire.PickAmmo(amount, magazine);
    }

    public override List<PW_Fire> GetFireModes() => [_primaryFire, _secondaryFire];

    public override void ResetBuffer()
    {
        _primaryFire.ResetBuffer();
        _secondaryFire.ResetBuffer();
    }
}