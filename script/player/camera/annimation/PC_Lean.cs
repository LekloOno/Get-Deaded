using Godot;

[GlobalClass]
public partial class PC_Lean : Node3D
{
    [ExportCategory("Settings")]
    [Export(PropertyHint.Range, "0.0,2.0,0.1")]
    public float AttackDamping {get; private set;} = 0.7f;

    [Export(PropertyHint.Range, "0.0,2.0,0.1")]
    public float DecayDamping {get; private set;} = 0.3f;

    [Export(PropertyHint.Range, "0.0,0.5,0.005")]
    public float Strength {get; private set;} = 0.03f;
    
    [ExportCategory("Setup")]
    [Export] private PC_Control _camera;
    [Export] private PM_Controller _controller;

    private Vector3 _dampedAcceleration;
    private Vector3 _dampedAccelerationVel;

    public override void _Process(double delta)
    {
        Vector3 planeAccel = MATH_Vector3Ext.Flat(_controller.Acceleration);
        float damping = planeAccel.Length() > _dampedAcceleration.Length()
            ? AttackDamping
            : DecayDamping;

        _dampedAcceleration = MATH_Vector3Ext.SmoothDamp
        (
            current: _dampedAcceleration,
            target: planeAccel,
            currentVelocity: ref _dampedAccelerationVel,
            smoothTime: damping,
            deltaTime: (float)delta
        );

        Vector3 leanAxis = _dampedAcceleration.Cross(_camera.GlobalBasis.Y).Normalized();
        Rotation = Vector3.Zero;

        if (!leanAxis.IsNormalized()) // Couldn't normalize axis
            return;

        GlobalRotate(leanAxis, _dampedAcceleration.Length() * Strength * 0.1f);
    }
}