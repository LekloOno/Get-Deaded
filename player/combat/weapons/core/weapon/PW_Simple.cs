using Godot;

[GlobalClass]
public partial class PW_Simple : PW_Weapon
{
    [Export] private PW_Fire _fire;
    [Export] private PW_SimpleADS _ads;

    public override void WeaponInitialize()
    {
        _ads?.Initialize(_camera);
        _fire.Initialize(_camera, _sight);
    }


    public override void PrimaryDown()
    {
        _fire.Press();
    }

    public override void PrimaryUp() {}

    public override void SecondaryDown()
    {
        if (_ads == null)
            return;

        if (_ads.ActivatedPress() is SimpleADSModifiers modifiers)
        {
            _fire.SpreadMultiplier = modifiers.Spread;
            _fire.RecoilMultiplier = modifiers.Recoil;
        }
    }

    public override void SecondaryUp()
    {
        if (_ads == null)
            return;

        if (_ads.ActivatedPress() is SimpleADSModifiers modifiers)
        {
            _fire.SpreadMultiplier = modifiers.Spread;
            _fire.RecoilMultiplier = modifiers.Recoil;
        }
    }
}