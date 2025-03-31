using System;
using Godot;

[GlobalClass]
public partial class PC_Spring : Node3D
{
    [ExportCategory("Settings")]
    [Export(PropertyHint.Range, "0.01,0.2,0.01,or_greater")]
    public float HalfLife {get; private set;} = 0.08f;

    [Export(PropertyHint.Range, "0.01,30.0,0.1,or_greater")]
    public float Frequency {get; private set;} = 14f;

    [Export(PropertyHint.Range, "0.01,15.0,0.1,or_greater")]
    public float AngularDisplacement {get; private set;} = 6f;

    [Export(PropertyHint.Range, "0.01,15.0,0.1,or_greater")]
    public float AngularZDisplacement {get; private set;} = 0.8f;

    [Export(PropertyHint.Range, "0.01,45.0,0.1,or_greater")]
    public float MaxAngularDisplacement {get; private set;} = 15f;
    [Export(PropertyHint.Range, "0.01,45.0,0.1,or_greater")]
    public float MaxAngularZDisplacement {get; private set;} = 15f;

    [Export(PropertyHint.Range, "0.0,0.3,0.01,or_greater")]
    public float LinearDisplacement {get; private set;} = 0.05f;
    [Export(PropertyHint.Range, "0.0,0.5,0.01,or_greater")]
    public float MaxLinearDisplacement {get; private set;} = 0.1f;
    
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

        float absSpringHeight = Mathf.Abs(springHeight);
        float signSpringHeight = Mathf.Sign(springHeight);
        float xAngularDisplacement = Mathf.Min(absSpringHeight * AngularDisplacement, MaxAngularDisplacement) * signSpringHeight;
        float zAngularDisplacement = Mathf.Min(absSpringHeight * AngularZDisplacement, MaxAngularZDisplacement) * signSpringHeight;
        RotationDegrees = new Vector3(xAngularDisplacement, 0f, zAngularDisplacement);

        Vector3 normalizedSpringPosition = localSpringPosition.Normalized();
        float linearDisplacementCoef = Mathf.Min(localSpringPosition.Length() * LinearDisplacement, MaxLinearDisplacement);
        GlobalPosition += normalizedSpringPosition * linearDisplacementCoef;
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