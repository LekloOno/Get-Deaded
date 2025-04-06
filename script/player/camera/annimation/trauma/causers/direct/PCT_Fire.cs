using Godot;

[GlobalClass]
public partial class PCT_Fire : PCT_DirectCauser
{
    [Export] private PW_Fire _fire;
    [Export] private float _amount;

    public override void _Ready() =>
        _fire.Shot += ShotTrauma;

    private void ShotTrauma(object sender, int e)=>
        _shakeableCamera.AddTrauma(_amount);
}