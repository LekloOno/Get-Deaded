using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class PW_Simple : PW_Weapon
{
    [Export] private PW_Fire _fire;

    protected override void SpecInitialize(PC_Shakeable shakeableCamera, PC_Recoil recoilController, GB_ExternalBodyManager owberBody)
    {
        _fire.Shot += (o, e) => Shot?.Invoke();
        _fire.Initialize(shakeableCamera, _sight, _barrel, recoilController, owberBody);
        _fire.Hit += (o, e) => Hit?.Invoke(o, e);
    }

    protected override void SpecDisable() => _fire.Disable();
    protected override bool SpecPrimaryPress() => _fire.HandlePress();
    protected override bool SpecPrimaryRelease() => _fire.HandleRelease();
    protected override bool SpecSecondaryPress() => true;
    protected override bool SpecSecondaryRelease() => true;

    protected override void SpecStartADS(){}
    protected override void SpecStopADS(){}

    protected override void SpecReload()
    {
        _fire.Reload();
    }
    protected override bool SpecCanReload(out bool tactical) => _fire.CanReload(out tactical);

    public override bool PickAmmo(int amount, bool magazine, int targetFireIndex) => _fire.PickAmmo(amount, magazine);
    public override List<PW_Fire> GetFireModes() => [_fire];
    public override void ResetBuffer() => _fire.ResetBuffer();

}