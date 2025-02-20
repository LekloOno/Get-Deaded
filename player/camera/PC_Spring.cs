using Godot;

[GlobalClass]
public partial class PC_Spring : Node3D
{
    [ExportCategory("Settings")]
    [Export(PropertyHint.Range, "0.01, 0.2")] public float HalfLife {get; private set;} = 0.08f;
    [Export(PropertyHint.Range, "0.01,30.0")] public float Frequency {get; private set;} = 14f;
    [Export(PropertyHint.Range, "0.01, 10.0")] public float AngularDisplacement {get; private set;} = 5f;
    [Export(PropertyHint.Range, "0.01, 0.2")] public float LinearDisplacement {get; private set;} = 0.05f;
    
    [ExportCategory("Setup")]
    [Export] private RemoteTransform3D _cameraTarget;
    
    private Vector3 _springPosition;
    private Vector3 _springVelocity;
    public override void _Ready()
    {
        _springPosition = GlobalPosition;
        _springVelocity = Vector3.Zero;
    }
    public override void _Process(double delta)
    {
        Position = Vector3.Zero;

        Spring((float)delta);
        
        Vector3 localSpringPosition = _springPosition - GlobalPosition;
        float springHeight = localSpringPosition.Dot(_cameraTarget.Basis.Y);
        RotationDegrees = new Vector3(-springHeight * AngularDisplacement, 0f, 0f);
        Position = localSpringPosition * LinearDisplacement;
    }

    //https://gist.github.com/keenanwoodall/951134976ad26a39e75b8b7643d026d6
    //https://github.com/TheAllenChou/numeric-springing
    private void Spring(float timeStep)
    {
        float dampingRatio = -Mathf.Log(0.5f) / (Frequency * HalfLife);
        float f = 1f + 2f * timeStep * dampingRatio * Frequency;
        float oo = Frequency * Frequency;
        float hoo = timeStep * oo;
        float hhoo = timeStep * hoo;
        float detInv = 1f / (f + hhoo);
        Vector3 detX = f * _springPosition + timeStep * _springVelocity + hhoo * GlobalPosition;
        Vector3 detV = _springVelocity + hoo * (GlobalPosition - _springPosition);
        _springPosition = detX * detInv;
        _springVelocity = detV * detInv;
    }
}