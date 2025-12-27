using Godot;

[GlobalClass]
public partial class PCT_SimpleTraumaData : Resource
{
    [Export] public float Amount {get; private set;}
    [Export] public bool Clamp {get; private set;} = true;

    public void AddTrauma(PC_Shakeable shakeableCamera)
    {
        if (Clamp)
            shakeableCamera.AddClampedTrauma(Amount, Amount);
        else
            shakeableCamera.AddTrauma(Amount);
    }
}