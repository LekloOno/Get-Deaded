using System;
using Godot;

[GlobalClass]
public partial class PM_Dash : Node
{
    [Export] private PI_Dash _dashInput;
    [Export] private PI_Walk _walkInput;
    [Export] private PC_Control _cameraControl;
    [Export] private PM_Controller _controller;
    [Export] private PS_Grounded _groundState;
    [Export] private PM_LedgeClimb _ledgeClimb;
    [Export] private PM_WallJump _wallJump;

    [Export(PropertyHint.Range, "0.0, 40.0")] private float _strength;
    [Export(PropertyHint.Range, "0.0, 1.0")] private float _dashDuration;
    [Export(PropertyHint.Range, "0.0, 1.0")] private float _minDashRatio;
    // The velocity coefficient when dashing upward. The more upward you dash, the less speed you will keep.

    private bool _available = true;
    private bool _isDashing = false;
    private Vector3 _prevVelocity = Vector3.Zero;
    private Vector3 _prevRealVelocity = Vector3.Zero;
    private Vector3 _dashForce = Vector3.Zero;
    private Vector3 _direction = Vector3.Zero;
    private SceneTreeTimer _endDashTimer;

    public override void _Ready()
    {
        _dashInput.OnStartInput += StartDash;
        _groundState.OnLanding += Reset;
        _wallJump.OnWallJump += Reset;
    }

    public void StartDash(object sender, EventArgs e)
    {
        if (!_available || _groundState.IsGrounded() || _ledgeClimb.IsClimbing)
            return;
        
        if (_walkInput.WalkAxis != Vector2.Zero)
            _direction = _walkInput.WishDir;
        else
            _direction = -_cameraControl.GlobalBasis.Z;

        _prevRealVelocity = _controller.RealVelocity;
        Vector3 velocity = _prevRealVelocity;
        //velocity = direction * velocity.Length();
        //_controller.RealVelocity = velocity;

        float appliedStrength = Mathf.Max(_strength, velocity.Length());
        _dashForce = appliedStrength * _direction;

        _controller.TakeOverForces.AddPersistent(_dashForce);
        _endDashTimer = GetTree().CreateTimer(_dashDuration);
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
        _controller.TakeOverForces.RemovePersistent(_dashForce);
        _isDashing = false;
        if (_endDashTimer != null)
        {
            _endDashTimer.Timeout -= EndDash;
            _endDashTimer = null;
        }
    }

    private void EndDash()
    {
        Vector3 outVelocity = OutVelocity();

        _controller.Velocity = outVelocity;
        _controller.RealVelocity = outVelocity;

        AbortDash();
    }

    private Vector3 OutVelocity()
    {
        // Angle ratio -
        // The more the dash is performed upward, the more speed you lose
        float angleRatio = _direction.Normalized().Dot(Vector3.Up);
        angleRatio = Mathf.Max(0, angleRatio);

        angleRatio = 1 + angleRatio * _minDashRatio - angleRatio;

        return _prevRealVelocity.Length() * angleRatio * _direction;
    }
}