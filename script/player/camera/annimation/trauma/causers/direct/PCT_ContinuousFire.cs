using Godot;

namespace Pew;

[GlobalClass]
public partial class PCT_ContinuousFire : PCT_Fire
{
    [Export] private float _max;
    public override void ShotTrauma(object sender, int e) =>
        _traumaLayer.AddClampedTrauma(_amount, _max);
}