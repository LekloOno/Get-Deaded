using System;
using System.ComponentModel.DataAnnotations.Schema;
using Godot;

[GlobalClass]
public partial class PM_LedgeClimb : Node
{
    [Export] public PI_Jump JumpInput {get; private set;}
    [Export] public PI_CrouchDispatcher CrouchInput {get; private set;}
    [Export] public PI_Walk WalkInput {get; private set;}
    [Export] public PM_Controller Controller {get; private set;}
    [Export] public PM_Dash Dash {get; private set;}
    [Export] public PM_Jump Jump {get; private set;}
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

    public override void _Ready() => SetPhysicsProcess(false);
    public override void _PhysicsProcess(double delta) => Climb();

    public Vector3 LedgeClimb(Vector3 velocity)
    {
        if (_isClimbing)        // Shouldn't happen ? to verify - might not neeed is climbing anymore
            return velocity;

        if (!ChestCast.IsColliding() || HeadCast.IsColliding())
            return Jump.Jump(velocity); // Propagate to Jump

        if (JumpInput.UseBuffer())
            DoLedgeClimb();             // Do it !

        return velocity;
    }

    private void DoLedgeClimb()
    {
            _startTime = Time.GetTicksMsec();
            _prevVelocity = Controller.Velocity;
            Dash.AbortDash();

            _direction = -ChestCast.GetCollisionNormal();
            _force = new Vector3(_direction.X*1.5f, ClimbSpeed, _direction.Z*1.5f);
            
            Controller.TakeOverForces.AddPersistent(_force);
            _isClimbing = true;
            SetPhysicsProcess(true);
    }


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

        Vector3 minOut = new(_direction.X, 1f, _direction.Z);

        Vector3 outVelocity = _prevVelocity;
        outVelocity = outVelocity.Max(minOut.Abs());
        outVelocity *= minOut.Sign();
        
        if (Time.GetTicksMsec() - CrouchInput.LastCrouchDown < SuperGlideWindow)
        {
            outVelocity += _direction * SuperGlideXStrength;
            outVelocity.Y = SuperGlideYStrength;
        }

        Controller.Velocity = outVelocity;
        Controller.RealVelocity = outVelocity;
        SetPhysicsProcess(false);
        _isClimbing = false;
    }
}