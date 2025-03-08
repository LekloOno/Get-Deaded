using System;
using Godot;

[GlobalClass]
public partial class PM_WallClimb : PM_Action
{
    [ExportCategory("Settings")]
    [Export] private float _minInWallSpeed = 3f;
    [Export] private float _speedDebuff = .1f;
    [Export] private float _jumpMinStrength = 1.5f;
    [Export] private float _jumpSpeedCoef = 0.4f;
    [Export] private float _hopMaxSpeed = 6f;
    [Export] private float _hopAccel = 1f;
    [Export] private float _maxVerticalSpeed = 3f;
    [Export] private int _maxClimbHops = 2;
    [Export] private float _betweenHopsTime = .4f;
    [Export] private float _hopDuration = .2f;

    [ExportCategory("Setup")]
    [Export] private Timer _startHopTimer;
    [Export] private PI_Jump _jumpInput;
    [Export] private PI_Walk _walkInput;

    [Export] private PM_Controller _controller;
    [Export] private PM_WallJump _wallJump;
    [Export] private PM_Jump _jump;
    [Export] private PM_LedgeClimb _ledgeClimb;
    [Export] private RayCast3D _headCast;

    private SceneTreeTimer _hopEndTimer;
    private bool _isWallClimbing = false;
    private int _currentHop;
    private bool _isHopping = false;

    public override void _Ready()
    {
        _startHopTimer.Timeout += WallHop;
        SetPhysicsProcess(false);
    }

    public Vector3 WallClimb(Vector3 velocity)
    {
        if (!_jumpInput.JumpDown || !_walkInput.IsForwarding())
            return _wallJump.WallJump(velocity);

        if (!IsCollidingWall(out Vector3 normal))
            return _ledgeClimb.LedgeClimb(velocity);

        if (_isWallClimbing)
            return velocity;
        
        Vector3 wallClimbVel = _controller.VelocityCache.UseCacheOr(velocity);
        float inWallSpeed = wallClimbVel.Dot(-normal);

        if (TooSlow(inWallSpeed) || _controller.RealVelocity.Y < 0)
            return _jump.Jump(velocity);

        StartWallClimb(inWallSpeed);
        return velocity - new Vector3(velocity.X, .0f, velocity.Z) * _speedDebuff;
    }

    private bool TooSlow(float inWallSpeed) => inWallSpeed < _minInWallSpeed;

    public bool IsCollidingWall(out Vector3 normal)
    {
        if (_headCast.IsColliding())
        {
            normal = _headCast.GetCollisionNormal();
            return true;
        }

        normal = Vector3.Zero;
        return false;
    }

    public void StartWallClimb(float inWallSpeed)
    {        
        Vector3 jumpForce = Vector3.Up * (_jumpMinStrength + inWallSpeed*_jumpSpeedCoef);
        _controller.AdditionalForces.AddImpulse(jumpForce);

        _currentHop = 0;
        _startHopTimer.Start(_betweenHopsTime);
        
        SetPhysicsProcess(true);

        _isWallClimbing = true;
        OnStart?.Invoke(this, EventArgs.Empty);
    }

    public void WallHop()
    {
        if (IsCollidingWall(out _) && _currentHop < _maxClimbHops)
            DoWallHop();
        else
            EndWallClimb();
    }

    private void DoWallHop()
    {
        GD.Print("hop");
        _isHopping = true;
        _hopEndTimer = GetTree().CreateTimer(_hopDuration);
        _hopEndTimer.Timeout += ResetWallClimbStep;

        Vector3 velFactor = new(_controller.Velocity.X, Math.Max(0, _controller.Velocity.Y), _controller.Velocity.Z);
        _controller.Velocity = velFactor;
        _currentHop += 1;
    }

    private void ResetWallClimbStep()
    {
        if (_hopEndTimer != null)
        {
            _isHopping = false;
            GD.Print("stop");
            _hopEndTimer.Timeout -= ResetWallClimbStep;
            _hopEndTimer = null;
        }
    }

    private void EndWallClimbStep()
    {

    }

    private void EndWallClimb()
    {
        _startHopTimer.Stop();
        ResetWallClimbStep();

        SetPhysicsProcess(false);

        _isWallClimbing = false;
        OnStop?.Invoke(this, EventArgs.Empty);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_ledgeClimb.CanLedgeClimb())
        {
            if(_isHopping)
            {
                float accel = Math.Clamp(_hopMaxSpeed - _controller.Velocity.Y, 0, _hopMaxSpeed*_hopAccel);
                _controller.AdditionalForces.AddImpulse(Vector3.Up * accel);
            }
            return;
        }
        
        EndWallClimb();
        
        Vector3 velCoef = new(1, 0, 1);
        _controller.Velocity *= velCoef;
        _controller.RealVelocity *= velCoef;
        _ledgeClimb.DoLedgeClimb();
    }
}