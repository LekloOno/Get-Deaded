using Godot;

[GlobalClass]
public partial class PWF_Ponctual : PW_Fire
{
    protected override bool Press()
    {
        bool didShoot = TryShoot();
        if (didShoot)
        {
            _recoilController.ResetBuffer();
            PC_RecoilHandler recoil = _recoilController.AddRecoil(new(0.6f, 5f), 0.15f);
            recoil.Completed += (o, e) => _recoilController.ResetRecoil(0.3f);
        }

        return didShoot;
    }
    protected override bool Release() => true;
    public override void Disable() {}
}