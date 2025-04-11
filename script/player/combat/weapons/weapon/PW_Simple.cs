using System.Collections.Generic;
using Godot;

[GlobalClass]
public partial class PW_Simple : PW_Weapon
{
    [Export] private PW_Fire _fire;
    private PW_SimpleADS _simpleADS;

    protected override void WeaponInitialize(PC_Shakeable shakeableCamera, PC_Recoil recoilController, GB_ExternalBodyManager owberBody)
    {
        _fire.Shot += (o, e) => Shot?.Invoke();
        _fire.Initialize(shakeableCamera, _camera, _sight, _barrel, recoilController, owberBody);
        _fire.Hit += (o, e) => Hit?.Invoke(o, e);

        if (_ads is PW_SimpleADS simpleADS)
            _simpleADS = simpleADS;
    }

    protected override void Disable() => _fire.Disable();
    protected override bool PrimaryDown() => _fire.HandlePress();
    protected override bool PrimaryUp() => _fire.HandleRelease();
    protected override bool SecondaryDown() => true;
    protected override bool SecondaryUp() => true;

    protected override void StartADS(){}

    protected override void StopADS(){}

    protected override void Reload()
    {
        _fire.Reload();
    }

    protected override bool CanReload(out bool tactical) => _fire.CanReload(out tactical);
    public override bool PickAmmo(int amount, bool magazine, int targetFireIndex) => _fire.PickAmmo(amount, magazine);

    public override List<PW_Fire> GetFireModes() => [_fire];
    public override void ResetBuffer() => _fire.ResetBuffer();
}