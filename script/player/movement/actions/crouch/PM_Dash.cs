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

    [Export(PropertyHint.Range, "0.0, 40.0")] private float _strength;
    [Export(PropertyHint.Range, "0.0, 1.0")] private float _dashDuration;
    [Export(PropertyHint.Range, "0.0, 1.0")] private float _minDashRatio;
    // The velocity coefficient when dashing upward. The more upward you dash, the less speed you will keep.
    [Export(PropertyHint.Range, "0.0, 10.0")] private float _cooldown;       // Only triggered when reseting from the same surface twice in a row (ground/wall)
    [Export] private float _slamWindow;

    public EventHandler<float> OnTryReset;
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
    private SceneTreeTimer _delayedResetTimer;


    public override void _Ready()
    {
        _dashInput.OnStartInput += StartDash;
        _groundState.OnLanding += LandReset;
        _wallJump.OnStart += WallReset;
    }

    public void StartDash(object sender, EventArgs e)
    {
        if (_groundState.IsGrounded() || _ledgeClimb.IsClimbing)
            return;

        if (!_available)
        {
            OnUnavailable?.Invoke(this, EventArgs.Empty);
            return;
        }
        
        if (PHX_Time.ScaledTicksMsec - _crouchDispatcher.LastCrouchDown <= _slamWindow)
            _direction = Vector3.Down;
        else if (_walkInput.WalkAxis != Vector2.Zero)
            _direction = _walkInput.WishDir;
        else
            _direction = -_cameraControl.GlobalBasis.Z;

        _prevRealVelocity = _controller.RealVelocity;
        Vector3 velocity = _prevRealVelocity;

        float appliedStrength = Mathf.Max(_strength, velocity.Length());
        _dashForce = appliedStrength * _direction;

        _controller.TakeOverForces.AddPersistent(_dashForce);
        _endDashTimer = GetTree().CreateTimer(_dashDuration, false, true);
        _endDashTimer.Timeout += EndDash;
        _isDashing = true;
        _available = false;

        _lastDash = PHX_Time.ScaledTicksMsec;
        OnStart?.Invoke(this, EventArgs.Empty);
    }

    public void TryReset(bool ground)
    {

        bool realLastGround = _lastGround;
        _lastGround = ground;
        
        if(_available)
            return;

        if(realLastGround ^ ground)
        {
            Reset();
            OnTryReset?.Invoke(this, 0f);
        }
        else
        {
            float sinceLastDash = (PHX_Time.ScaledTicksMsec - _lastDash)/1000f;
            float remaining = _cooldown - sinceLastDash;

            if (remaining <= 0)
            {
                Reset();
                OnTryReset?.Invoke(this, 0f);
            }
            else if (_delayedResetTimer == null)
            {
                _delayedResetTimer = GetTree().CreateTimer(remaining, false, true);
                _delayedResetTimer.Timeout += Reset;
                OnTryReset?.Invoke(this, remaining);
            }
        }
    }

    public void WallReset(object sender, EventArgs e) => TryReset(false);
    public void LandReset(object sender, EventArgs e) => TryReset(true);

    private void Reset()
    {
        if (_delayedResetTimer != null)
        {
            _delayedResetTimer.Timeout -= Reset;
            _delayedResetTimer = null;
        }
        else if (_isDashing)
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