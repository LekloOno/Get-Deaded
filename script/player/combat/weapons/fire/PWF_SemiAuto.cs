using Godot;

[GlobalClass]
public partial class PWF_SemiAuto : PW_Fire
{
    protected override bool SpecPress()
    {
        bool didShoot = TryShoot();
        if (didShoot)
            _recoil?.Start();

        return didShoot;
    }
    protected override bool SpecRelease() => true;
    public override void Disable() {}

    protected override void SpecInitialize(PC_Shakeable shakeableCamera, PC_Recoil recoilController, GB_ExternalBodyManager ownerBody){}
}