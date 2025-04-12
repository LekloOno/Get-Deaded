using Godot;

[GlobalClass]
public partial class PWF_Ponctual : PW_Fire
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
}