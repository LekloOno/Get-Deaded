using Godot;

[GlobalClass]
public partial class PW_Simple : PW_Weapon
{
    [Export] private PW_Fire _fire;
    private PW_SimpleADS _simpleADS;

    protected override void WeaponInitialize()
    {
        _fire.Initialize(_camera, _sight, _barel);
        _fire.Hit += (o, e) => Hit?.Invoke(o, e);

        if (_ads is PW_SimpleADS simpleADS)
            _simpleADS = simpleADS;
    }

    protected override void Disable() => _fire.Disable();
    protected override void PrimaryDown() => _fire.HandlePress();
    protected override void PrimaryUp() => _fire.HandleRelease();
    protected override void SecondaryDown() {}
    protected override void SecondaryUp() {}

    protected override void StartADS()
    {
        if(_simpleADS == null)
            return;

        _fire.SpreadMultiplier.AddModifier(_simpleADS.SpreadMultiplier);
        _fire.RecoilMultiplier.AddModifier(_simpleADS.RecoilMultiplier);
    }

    protected override void StopADS()
    {
        if(_simpleADS == null)
            return;

        _fire.SpreadMultiplier.RemoveModifier(_simpleADS.SpreadMultiplier);
        _fire.RecoilMultiplier.RemoveModifier(_simpleADS.RecoilMultiplier);
    }

}