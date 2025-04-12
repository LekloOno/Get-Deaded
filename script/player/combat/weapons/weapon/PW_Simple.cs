using System;
using Godot;

[GlobalClass]
public partial class PW_Simple : PW_Weapon
{
    private PW_Fire Fire => _fires[0];

    protected override void SpecInitialize(PC_Shakeable shakeableCamera, Node3D sight, PC_Recoil recoilController, GB_ExternalBodyManager owberBody)
    {
        int firesCount = _fires.Count;
        if (firesCount == 0)
            throw new InvalidOperationException($"Simple weapons must have exactly 1 fire mode, but none are assigned.");
        else if (firesCount > 1)
            GD.PushWarning($"Simple weapons expects 1 fire mode, but {firesCount} are assigned. The extra modes will be ignored.");
    }

    protected override bool SpecSecondaryPress() => true;
    protected override bool SpecSecondaryRelease() => true;

    protected override void SpecStartADS(){}
    protected override void SpecStopADS(){}

    protected override bool SpecCanReload(out bool tactical) => Fire.CanReload(out tactical);

    public override bool PickAmmo(int amount, bool magazine, int targetFireIndex) => Fire.PickAmmo(amount, magazine);

}