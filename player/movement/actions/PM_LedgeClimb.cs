using System;
using Godot;

[GlobalClass]
public partial class PM_LedgeClimb : Node
{
    [Export] public PI_Jump JumpInput {get; private set;}
    [Export] public PI_CrouchDispatcher CrouchInput {get; private set;}
    [Export] public PI_Walk WalkInput {get; private set;}
    [Export] public PM_Controller Controller {get; private set;}
    [Export] public PM_Dash Dash {get; private set;}
    [Export] public RayCast3D HeadCast {get; private set;}
    [Export] public RayCast3D ChestCast {get; private set;}
    [Export] public RayCast3D FootCast {get; private set;}

    [Export(PropertyHint.Range, "0.0, 2.0")] public float MaxClimbTime {get; private set;}
    [Export(PropertyHint.Range, "1.0,20.0")] public float ClimbSpeed {get; private set;}
    [Export(PropertyHint.Range, "  0,1000")] public ulong SuperGlideWindow {get; private set;}
    [Export(PropertyHint.Range, "0.0,10.0")] public float SuperGlideYStrength {get; private set;}
    [Export(PropertyHint.Range, "0.0,10.0")] public float SuperGlideXStrength {get; private set;}
    private ulong _startTime = 0;
    private bool _isClimbing = false;
    public bool IsClimbing => _isClimbing;
    private Vector3 _force = Vector3.Zero;
    private Vector3 _prevVelocity = Vector3.Zero;
    private Vector3 _direction = Vector3.Zero;

    public override void _Ready()
    {
        SetPhysicsProcess(false);
        JumpInput.OnStartInput += TryLedgeClimb;
    }

    private void TryLedgeClimb(object sender, EventArgs e)
    {
        if (!_isClimbing && ChestCast.IsColliding() && !HeadCast.IsColliding())
        {
            Dash.AbortDash();
            _direction = -ChestCast.GetCollisionNormal() * 5f;
            _prevVelocity = Controller.Velocity;
            JumpInput.UseBuffer();
            _startTime = Time.GetTicksMsec();
            _force = new Vector3(_direction.X, ClimbSpeed, _direction.Z);
            Controller.TakeOverForces.AddPersistent(_force);
            SetPhysicsProcess(true);
            _isClimbing = true;
        }
    }

    public override void _PhysicsProcess(double delta) => Climb();

    private void Climb()
    {
        float timeElapsed = (Time.GetTicksMsec() - _startTime)/1000f;
        if (timeElapsed > MaxClimbTime || !FootCast.IsColliding())
            StopClimb();
    }

    private void StopClimb()
    {
        _startTime = 0;
        Controller.TakeOverForces.RemovePersistent(_force);

        Vector3 outVelocity = _prevVelocity;
        if (Time.GetTicksMsec() - CrouchInput.LastCrouchDown < SuperGlideWindow)
        {
            GD.Print("Superglide !");
            outVelocity += _direction.Normalized() * SuperGlideXStrength;
            outVelocity.Y = SuperGlideYStrength;
        }

        Controller.Velocity = outVelocity;
        Controller.RealVelocity = outVelocity;
        SetPhysicsProcess(false);
        _isClimbing = false;
    }
}