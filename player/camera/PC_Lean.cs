using Godot;

[GlobalClass]
public partial class PC_Lean : Node3D
{
    [Export] public Camera3D Camera {get; private set;}
    [Export] public PM_Controller Controller {get; private set;}
    [Export(PropertyHint.Range, "0.0, 2.0")] public float AttackDamping {get; private set;} = 0.5f;
    [Export(PropertyHint.Range, "0.0, 2.0")] public float DecayDamping {get; private set;} = 0.3f;
    [Export(PropertyHint.Range, "0.0, 2.0")] public float Strength {get; private set;} = 0.075f;
    

    private Vector3 _dampedAcceleration;
    private Vector3 _dampedAccelerationVel;
    
    public override void _Ready()
    {
        // To implement
    }
    public override void _PhysicsProcess(double delta)
    {
        // To implement
    }
    public override void _Process(double delta)
    {
        GD.Print(Controller.Acceleration.Length());

        //Vector3 planeAccel = Controller.Acceleration.Project(Camera.Basis.Y);
        Vector3 planeAccel = PHX_Vector3Ext.Flat(Controller.Acceleration);
        float damping = planeAccel.Length() > _dampedAcceleration.Length()
            ? AttackDamping
            : DecayDamping;

        _dampedAcceleration = PHX_Vector3Ext.SmoothDamp
        (
            current: _dampedAcceleration,
            target: planeAccel,
            currentVelocity: ref _dampedAccelerationVel,
            smoothTime: damping,
            deltaTime: (float)delta
        );

        Vector3 leanAxis = _dampedAcceleration.Cross(Camera.Basis.Y).Normalized();
        Rotation = Vector3.Zero;

        if (leanAxis == Vector3.Zero)
            return;

        Rotate(leanAxis, _dampedAcceleration.Length() * Strength * 0.1f);
    }
}