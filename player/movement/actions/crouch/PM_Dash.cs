using System;
using Godot;

[GlobalClass]
public partial class PM_Dash : PM_Action
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
    [Export(PropertyHint.Range, "0.0, 2.0")] private float _cooldown;       // Only triggered when reseting from the same surface twice in a row (ground/wall)

    private bool _available = true;
    private bool _isDashing = false;
    private bool _lastResetGround = true; // Determine wether the last direct dash reset was due to landing or wall jumping
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

        _lastDash = Time.GetTicksMsec();
        OnStart?.Invoke(this, EventArgs.Empty);
    }

    public void TryReset(bool ground)
    {
        if(_available)
            return;

        if(_lastResetGround ^ ground)
        {
            Reset();
            _lastResetGround = ground;
            //GD.Print("direct reset");
        }
        else
        {
            float sinceLastDash = (Time.GetTicksMsec() - _lastDash)/1000f;
            float remaining = _cooldown - sinceLastDash;

            if (remaining < 0)
                Reset();
            else
            {
                _delayedResetTimer = GetTree().CreateTimer(remaining);
                _delayedResetTimer.Timeout += Reset;
                //GD.Print("delayed reset - " + remaining);
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