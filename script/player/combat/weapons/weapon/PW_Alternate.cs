using System;
using Godot;

[GlobalClass]
public partial class PW_Alternate : PW_Weapon
{
    private PW_Fire PrimaryFire => _fires[0];
    private PW_Fire SecondaryFire => _fires[1];

    protected override void SpecInitialize(PC_Shakeable shakeableCamera, Node3D sight, PC_Recoil recoilController, GB_ExternalBodyManager owberBody)
    {
        int firesCount = _fires.Count;
        if (firesCount < 2)
            throw new InvalidOperationException($"Alternate weapons must have exactly 2 fire modes, but {firesCount} are assigned.");
        else if (firesCount > 2)
            GD.PushWarning($"Alternate weapons expects 2 fire modes, but {firesCount} are assigned. The extra modes will be ignored.");
    }

    protected override bool SpecSecondaryPress() => SecondaryFire.Press();
    protected override bool SpecSecondaryRelease() => SecondaryFire.Release();


    protected override void SpecStartADS()
    {
        _currentFire.Disable();
        _currentFire = SecondaryFire;
    }

    protected override void SpecStopADS()
    {
        _currentFire.Disable();
        _currentFire = PrimaryFire;
    }

    public override bool PickAmmo(int amount, bool magazine, int targetFireIndex)
    {
        if (targetFireIndex == 0)
        {
            int distribAmount = amount / 2;
            int remainder = amount % 2;
            bool primary = PrimaryFire.PickAmmo(distribAmount + remainder, magazine);
            bool secondary = SecondaryFire.PickAmmo(distribAmount, magazine);
            return primary || secondary;
        }

        int realTargetIndex = targetFireIndex - 1;
        if ((realTargetIndex & 1) == 0)
            return PrimaryFire.PickAmmo(amount, magazine);

        return SecondaryFire.PickAmmo(amount, magazine);
    }
}