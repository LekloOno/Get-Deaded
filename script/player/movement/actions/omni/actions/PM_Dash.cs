using System;
using Godot;

[GlobalClass]
public partial class PM_Dash : PM_Action
{
    [Export] private PI_CrouchDispatcher _crouchDispatcher;
    [Export] private PI_Dash _dashInput;
    [Export] private PI_Walk _walkInput;
    [Export] private PC_Control _cameraControl;
    [Export] private PM_Controller _controller;
    [Export] private PS_Grounded _groundState;
    [Export] private PM_LedgeClimb _ledgeClimb;
    [Export] private PM_WallJump _wallJump;
    [Export] private PM_OmniCharge _charge;
    [Export] private float _chargeCost = 100f;

    private float _distance = 7;
    [Export(PropertyHint.Range, "0.0, 10.0, or_greater")] public float Distance
    {
        get => _distance;
        set
        {
            _distance = value;
            SetDashSpeed();
        }
    }
    private float _speed;
    private float _dashDuration = 0.1f;
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
    [Export(PropertyHint.Range, "0.0, 1.0")] private float _minDashRatio;
    // The velocity coefficient when dashing upward. The more upward you dash, the less speed you will keep.
    [Export(PropertyHint.Range, "0.0, 10.0, or_greater")] private float _cooldown;       // Only triggered when reseting from the same surface twice in a row (ground/wall)
    [Export] private float _slamWindow;

    public EventHandler OnUnavailable;

    private bool _available = true;
    private bool _isDashing = false;
    private bool _lastGround = true;    // True if the last surface is a wall or ground. Wall is specifically a wall jump, ground is simply landing.
    private ulong _lastDash = 0;
    private Vector3 _prevVelocity = Vector3.Zero;
    private Vector3 _prevRealVelocity = Vector3.Zero;
    private Vector3 _dashForce = Vector3.Zero;
    private Vector3 _direction = Vector3.Zero;
    private SceneTreeTimer _endDashTimer;


    public override void _Ready()
    {
        SetDashSpeed();
        _dashInput.OnStartInput += StartDash;
    }

    private void SetDashSpeed() =>
        _speed = _distance/_dashDuration;

    public void StartDash(object sender, EventArgs e)
    {
        if (_ledgeClimb.IsClimbing)
            return;

        if (_isDashing)
            return;

        if (!_charge.TryConsume(_chargeCost))
            return;
        
        _direction = GetDashDirection();

        _prevRealVelocity = _controller.RealVelocity;
        Vector3 velocity = _prevRealVelocity;

        float appliedSpeed = Mathf.Max(_speed, velocity.Length());
        _dashForce = appliedSpeed * _direction;

        _controller.TakeOverForces.AddPersistent(_dashForce);
        _endDashTimer = GetTree().CreateTimer(_dashDuration, false, true);
        _endDashTimer.Timeout += EndDash;
        _isDashing = true;

        _lastDash = PHX_Time.ScaledTicksMsec;

        _controller.CollisionMask = CONF_Collision.Layers.Environment;
        OnStart?.Invoke(this, EventArgs.Empty);
    }

    private Vector3 GetDashDirection() =>
        GetDashDirection(
            _walkInput.WalkAxis,
            _walkInput.WishDir,
            -_cameraControl.GlobalBasis.Z,
            _walkInput.FlatDir
        );
        
    private Vector3 GetDashDirection(Vector2 walkAxis, Vector3 wishDir, Vector3 dir, Vector3 flatDir)
    {
        if (IsSlam())
            return Vector3.Down;
        
        if (walkAxis != Vector2.Zero)
            return wishDir;
        
        if (!_groundState.IsGrounded())
            return dir;
        
        return flatDir;
    }

    private bool IsSlam() =>
        PHX_Time.ScaledTicksMsec - _crouchDispatcher.LastCrouchDown <= _slamWindow;

    public void AbortDash()
    {
        _controller.TakeOverForces.RemovePersistent(_dashForce);
        _isDashing = false;
        _controller.CollisionMask = CONF_Collision.Masks.Environment;
        if (_endDashTimer != null)
        {
            _endDashTimer.Timeout -= EndDash;
            _endDashTimer = null;
        }

        OnStop?.Invoke(this, EventArgs.Empty);
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