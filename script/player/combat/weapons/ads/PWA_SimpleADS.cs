using Godot;

namespace Pew;

[GlobalClass]
public partial class PWA_SimpleADS : PW_ADS
{
    [Export] private float _spreadMultiplier = 0f;
    [Export] private float _recoilMultiplier = 0f;

    [Export] private PW_Fire _fire;

    protected override void SpecActivate()
    {
        _fire.AddSpread(_spreadMultiplier);
        _fire?.RecoilMultiplier.Add(_recoilMultiplier);
    }
    protected override void SpecDeactivate()
    {
        _fire.RemoveSpread(_spreadMultiplier);
        _fire?.RecoilMultiplier.Remove(_recoilMultiplier);
    }
}