using Godot;

[GlobalClass]
public partial class PC_Recoil : Node3D
{
    [Export] public Vector2 Resistance {get; private set;}
    [Export] public float StopThreshold {get; private set;}
    [Export] public PC_Control CameraControl {get; private set;}
    private PC_SimpleRecoil _simpleRecoil;
    public override void _Ready()
    {
        _simpleRecoil = new(this);
        AddChild(_simpleRecoil);
    }

    public void AddRecoil(Vector2 velocity)
    {
        _simpleRecoil.AddRecoil(velocity);
        _simpleRecoil.SetProcess(true);
    }

    public void AddCappedRecoil(Vector2 velocity, float max)
    {
        _simpleRecoil.AddCappedRecoil(velocity, max);
        _simpleRecoil.SetProcess(true);
    }

    public void RecoilAndReset(Vector2 recoilVelocity)
    {
        
    }
}