using System;
using Godot;

[GlobalClass]
public partial class PM_Dash : PM_Action
{
    [Export] private PI_Dash       _dashInput     = null!;
    [Export] private PI_Walk       _walkInput     = null!;
    [Export] private PC_Control    _cameraControl = null!;
    [Export] private PM_Controller _controller    = null!;
    [Export] private PM_LedgeClimb _ledgeClimb    = null!;
    [Export] private PM_OmniCharge _charge        = null!;
    [Export] private float         _dashCost      = 90f;

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

    public event Action? OnUnavailable;

    private bool _available = true;
    private bool _isDashing = false;
    private bool _lastGround = true;    // True if the last surface is a wall or ground. Wall is specifically a wall jump, ground is simply landing.
    private ulong _lastDash = 0;
    private Vector3 _prevVelocity = Vector3.Zero;
    private Vector3 _prevRealVelocity = Vector3.Zero;
    private Vector3 _force = Vector3.Zero;
    private Vector3 _direction = Vector3.Zero;
    private SceneTreeTimer? _endDashTimer;


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

        if (!_charge.TryConsume(_dashCost))
        {
            OnUnavailable?.Invoke();
            return;
        }

        _prevRealVelocity = _controller.RealVelocity;
        Vector3 velocity = _prevRealVelocity;

        _direction = GetDashDirection();
        float appliedSpeed = Mathf.Max(_dashSpeed, velocity.Length());
        _force = appliedSpeed * _direction;
        float duration = _dashDuration;
        _controller.TakeOverForces.AddPersistent(_force);
        InvokeStart();

        _endDashTimer = GetTree().CreateTimer(duration, false, true);
        _endDashTimer.Timeout += EndDash;
        _isDashing = true;

        _lastDash = PHX_Time.ScaledTicksMsec;

        _controller.CollisionMask = CONF_Collision.Layers.Environment;
        
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
        if (walkAxis != Vector2.Zero)
            return wishDir;

        return flatDir;
    }

    public void AbortDash()
    {
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