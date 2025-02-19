using System;
using Godot;

[GlobalClass]
public partial class PM_Dash : Node
{
    [Export] public PI_Dash DashInput {get; private set;}
    [Export] public PI_Walk WalkInput {get; private set;}
    [Export] public PC_Control CameraControl {get; private set;}
    [Export] public PM_Controller Controller {get; private set;}
    [Export] public PS_Grounded GroundState {get; private set;}
    [Export] public PM_LedgeClimb LedgeClimb {get; private set;}

    [Export(PropertyHint.Range, "0.0, 40.0")] public float Strength {get; private set;}
    [Export(PropertyHint.Range, "0.0, 1.0")] public float DashDuration {get; private set;}

    private bool _available = true;
    private bool _isDashing = false;
    private Vector3 _prevVelocity = Vector3.Zero;
    private Vector3 _prevRealVelocity = Vector3.Zero;
    private Vector3 _dashForce = Vector3.Zero;
    private Vector3 _direction = Vector3.Zero;
    private SceneTreeTimer _endDashTimer;

    public override void _Ready()
    {
        DashInput.OnStartInput += StartDash;
        GroundState.OnLanding += Reset;
    }

    public void StartDash(object sender, EventArgs e)
    {
        if (!_available || GroundState.IsGrounded() || LedgeClimb.IsClimbing)
            return;
        
        if (WalkInput.WalkAxis != Vector2.Zero)
            _direction = WalkInput.WishDir;
        else
            _direction = -CameraControl.GlobalBasis.Z;

        _prevRealVelocity = Controller.RealVelocity;
        Vector3 velocity = _prevRealVelocity;
        //velocity = direction * velocity.Length();
        //Controller.RealVelocity = velocity;

        float appliedStrength = Mathf.Max(Strength, velocity.Length());
        _dashForce = appliedStrength * _direction;

        Controller.TakeOverForces.AddPersistent(_dashForce);
        _endDashTimer = GetTree().CreateTimer(DashDuration);
        _endDashTimer.Timeout += EndDash;
        _isDashing = true;
        _available = false;
    }

    public void Reset(object sender, EventArgs e)
    {
        if(_isDashing) 
            EndDash();

        _available = true;
    }

    public void AbortDash()
    {
        Controller.TakeOverForces.RemovePersistent(_dashForce);
        _isDashing = false;
        if (_endDashTimer != null)
        {
            _endDashTimer.Timeout -= EndDash;
            _endDashTimer = null;
        }
    }

    private void EndDash()
    {
        Controller.Velocity = _direction * _prevRealVelocity.Length();
        Controller.RealVelocity = _direction * _prevRealVelocity.Length();

        AbortDash();
    }
}