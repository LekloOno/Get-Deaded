using System;
using Godot;

[GlobalClass]
public partial class PM_Dash : PM_Action
{
    [Export] private PI_CrouchDispatcher _crouchDispatcher;
    [Export] private PI_Dash _dashInput;
    [Export] private PI_Walk _walkInput;
    [Export] private PI_Jump _jumpInput;
    [Export] private PC_Control _cameraControl;
    [Export] private PM_Controller _controller;
    [Export] private PS_Grounded _groundState;
    [Export] private PM_LedgeClimb _ledgeClimb;
    [Export] private PM_WallJump _wallJump;
    [Export] private PM_OmniCharge _charge;
    [Export] private float _dashCost = 90f;
    [Export] private float _slamCost = 60f;
    [Export] private float _doubleJumpCost = 60f;

    private bool _wasDoubleJump = false;

    private float _dashDistance = 6.5f;
    [Export(PropertyHint.Range, "0.0, 10.0, or_greater")] public float Distance
    {
        get => _dashDistance;
        set
        {
            _dashDistance = value;
            SetDashSpeed();
        }
    }
    private float _dashSpeed;
    private float _dashDuration = 0.09f;
    [Export(PropertyHint.Range, "0.01, 0.5, or_greater")]
    public float DashDuration
    {
        get => _dashDuration;
        set
        {
            _dashDuration = value;
            SetDashSpeed();
        }
    }
    [Export] private float _slamWindow;

    [Export] private float _doubleJumpStrength = 5f;
    [Export] private float _doubleJumpPenalty = 0.1f;
    [Export] private float _doubleJumpVerticalPenalty = 0.5f;
    [Export] private float _doubleJumpDuration;

    public EventHandler OnUnavailable;

    private bool _available = true;
    private bool _isDashing = false;
    private bool _lastGround = true;    // True if the last surface is a wall or ground. Wall is specifically a wall jump, ground is simply landing.
    private ulong _lastDash = 0;
    private Vector3 _prevVelocity = Vector3.Zero;
    private Vector3 _prevRealVelocity = Vector3.Zero;
    private Vector3 _force = Vector3.Zero;
    private Vector3 _direction = Vector3.Zero;
    private SceneTreeTimer _endDashTimer;


    public override void _Ready()
    {
        SetDashSpeed();
        _dashInput.OnStartInput += StartDash;
    }

    private void SetDashSpeed() =>
        _dashSpeed = _dashDistance/_dashDuration;

    public void StartDash(object sender, EventArgs e)
    {
        if (_ledgeClimb.IsClimbing)
            return;

        if (_isDashing)
            return;

        float cost = GetChargeCost();

        if (!_charge.TryConsume(cost))
            return;

        _prevRealVelocity = _controller.RealVelocity;
        Vector3 velocity = _prevRealVelocity;
        float duration;

        _wasDoubleJump = IsDoubleJump();

        if (_wasDoubleJump)
        {
            float horPenalty = 1 - _doubleJumpPenalty;
            float vertPenalty = 1 - _doubleJumpVerticalPenalty;
            
            velocity *= new Vector3(horPenalty, vertPenalty, horPenalty);

            //velocity.Y = _doubleJumpStrength;
            _prevRealVelocity = velocity;

            _controller.RealVelocity = velocity;
            _controller.Velocity = velocity;

            _direction = velocity.Normalized();
            _force = _doubleJumpStrength * Vector3.Up;
            duration = _doubleJumpDuration;
            _controller.AdditionalForces.AddPersistent(_force);
        } else
        {
            _direction = GetDashDirection();
            float appliedSpeed = Mathf.Max(_dashSpeed, velocity.Length());
            _force = appliedSpeed * _direction;
            duration = _dashDuration;
            _controller.TakeOverForces.AddPersistent(_force);
        }

        _endDashTimer = GetTree().CreateTimer(duration, false, true);
        _endDashTimer.Timeout += EndDash;
        _isDashing = true;

        _lastDash = PHX_Time.ScaledTicksMsec;

        _controller.CollisionMask = CONF_Collision.Layers.Environment;
        InvokeStart();
    }

    private Vector3 GetDashDirection() =>
        GetDashDirection(
            _walkInput.WalkAxis,
            _walkInput.WishDir,
            -_cameraControl.GlobalBasis.Z,
            _walkInput.FlatDir
        );

    private float GetChargeCost()
    {
        if (_groundState.IsGrounded())
            return _dashCost;

        if (IsSlam())
            return _slamCost;

        if (IsDoubleJump())
            return _doubleJumpCost;

        return _dashCost;
    }
        
    private Vector3 GetDashDirection(Vector2 walkAxis, Vector3 wishDir, Vector3 dir, Vector3 flatDir)
    {
        if (!_groundState.IsGrounded() && IsSlam())
            return Vector3.Down;
        
        if (walkAxis != Vector2.Zero)
            return wishDir;

        return flatDir;
    }

    private bool IsDoubleJump() =>
        PHX_Time.ScaledTicksMsec - _jumpInput.LastInput <= _slamWindow;

    private bool IsSlam() =>
        PHX_Time.ScaledTicksMsec - _crouchDispatcher.LastCrouchDown <= _slamWindow;

    public void AbortDash()
    {
        if (_wasDoubleJump)
            _controller.AdditionalForces.RemovePersistent(_force);
        else
            _controller.TakeOverForces.RemovePersistent(_force);

        _isDashing = false;
        _controller.CollisionMask = CONF_Collision.Masks.Environment;
        if (_endDashTimer != null)
        {
            _endDashTimer.Timeout -= EndDash;
            _endDashTimer = null;
        }

        InvokeStop();
    }

    private void EndDash()
    {
        if (_wasDoubleJump)
        {
            AbortDash();
            return;
        }

        Vector3 outVelocity = OutVelocity();

        _controller.Velocity = outVelocity;
        _controller.RealVelocity = outVelocity;

        AbortDash();
    }

    private Vector3 OutVelocity()
    {
        // Angle ratio -
        // The more the dash is performed upward, the more speed you lose
        return _prevRealVelocity.Length() * _direction;
    }
}