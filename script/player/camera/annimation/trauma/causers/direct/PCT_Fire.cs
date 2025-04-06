using Godot;

[GlobalClass]
public partial class PCT_Fire : PCT_DirectCauser
{
    [Export] private PW_Fire _fire;
    [Export] protected PC_TraumaLayer _traumaLayer;
    [Export] protected float _amount;

    public override void _Ready()
    {
        _shakeableCamera.AddLayer(_traumaLayer);
        _fire.Shot += ShotTrauma;
    }

    protected virtual void ShotTrauma(object sender, int e)=>
        _traumaLayer.AddTrauma(_amount);
}