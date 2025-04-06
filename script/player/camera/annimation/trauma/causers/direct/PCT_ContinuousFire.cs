using Godot;

[GlobalClass]
public partial class PCT_ContinuousFire : PCT_Fire
{
    [Export] private float _max;
    protected override void ShotTrauma(object sender, int e) =>
        _shakeableCamera.AddClampedTrauma(_amount, _max);
}