using Godot;

[GlobalClass]
public partial class PW_SimpleADS : PW_ADS
{
    [Export] private float _spreadMultiplier = 0f;
    [Export] private float _recoilMultiplier = 0f;

    [Export] private PW_Fire _fire;

    protected override void SpecActivate()
    {
        _fire.SpreadMultiplier.Add(_spreadMultiplier);
        _fire?.RecoilMultiplier.Add(_recoilMultiplier);
    }
    protected override void SpecDeactivate()
    {
        _fire.SpreadMultiplier.Remove(_spreadMultiplier);
        _fire?.RecoilMultiplier.Remove(_recoilMultiplier);
    }
}