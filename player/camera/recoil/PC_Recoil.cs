using Godot;

[GlobalClass]
public partial class PC_Recoil : Node3D
{
    [Export] public Vector2 Resistance {get; private set;}
    [Export] public float StopThreshold {get; private set;}
    [Export] public PC_Control CameraControl {get; private set;}
    [Export] private float _resetSpeed;
    private PC_SimpleRecoil _simpleRecoil;
    private PC_ResetRecoil _resetRecoil;
    public override void _Ready()
    {
        _simpleRecoil = new(this);
        AddChild(_simpleRecoil);

        _resetRecoil = new(this, _resetSpeed);
        AddChild(_resetRecoil);
    }

    public void AddRecoil(Vector2 velocity, bool reset)
    {
        if (reset)
        {
            _resetRecoil.AddRecoil(velocity);
            _resetRecoil.SetProcess(true);
        }
        else
        {
            _simpleRecoil.AddRecoil(velocity);
            _simpleRecoil.SetProcess(true);
        }
    }

    public void AddCappedRecoil(Vector2 velocity, float max, bool reset)
    {
        if (reset)
        {
            _resetRecoil.AddCappedRecoil(velocity, max);
            _resetRecoil.SetProcess(true);
        }
        else
        {
            _simpleRecoil.AddCappedRecoil(velocity, max);
            _simpleRecoil.SetProcess(true);
        }
    }
}