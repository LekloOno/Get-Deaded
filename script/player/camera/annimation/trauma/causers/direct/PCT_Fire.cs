using Godot;

namespace Pew;

[GlobalClass]
public partial class PCT_Fire : PCT_DirectCauser
{
    [Export] protected PC_TraumaLayer _traumaLayer;
    [Export] protected float _amount;

    protected override void InnerInitialize()
    {
        _shakeableCamera.AddLayer(_traumaLayer);
    }

    public virtual void ShotTrauma(object sender, int e)=>
        _traumaLayer.AddTrauma(_amount);
}