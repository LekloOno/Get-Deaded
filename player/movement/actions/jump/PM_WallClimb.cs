using System;
using Godot;

[GlobalClass]
public partial class PM_WallClimb : PM_Action
{
    [ExportCategory("Settings")]
    [Export] private float _jumpMinForce = 3f;
    [Export] private int _maxClimbHops = 3;

    [ExportCategory("Setup")]
    [Export] private Timer _endClimbTimer;
    [Export] private PI_Jump _jumpInput;
    [Export] private PI_Walk _walkInput;

    [Export] private PM_Controller _controller;
    [Export] private PM_WallJump _wallJump;
    [Export] private PM_LedgeClimb _ledgeClimb;
    [Export] private RayCast3D _headCast;

    private Vector3 _nextHopForce = Vector3.Zero;
    private bool _isWallClimbing;
    private int _currentIteration;

    public override void _Ready()
    {
        _endClimbTimer.Timeout += WallHop;
        SetPhysicsProcess(false);
    }

    public Vector3 WallClimb(Vector3 velocity)
    {
        //GD.Print(_isWallClimbing);

        if (!_jumpInput.JumpDown || !_walkInput.IsForwarding())
            return _wallJump.WallJump(velocity);

        if (!IsCollidingWall(out Vector3 normal))
            return _ledgeClimb.LedgeClimb(velocity);

        if (_isWallClimbing)
            return velocity;
        
        Vector3 wallClimbVel = _controller.VelocityCache.UseCacheOr(velocity);
        StartWallClimb(wallClimbVel, normal);
        return velocity;
    }

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

    public void StartWallClimb(Vector3 velocity, Vector3 normal)
    {
        float inWallSpeed = velocity.Dot(-normal);
        _nextHopForce = Vector3.Up * (_jumpMinForce + inWallSpeed);

        _currentIteration = 0;

        WallHop();
        _endClimbTimer.Start();
        SetPhysicsProcess(true);

        _isWallClimbing = true;
        OnStart?.Invoke(this, EventArgs.Empty);
    }

    public void WallHop()
    {
        if (IsCollidingWall(out _) && _currentIteration < _maxClimbHops)
            DoWallHop();
        else
            EndWallClimb();
    }

    private void DoWallHop()
    {
        _controller.AdditionalForces.AddImpulse(_nextHopForce);
        _currentIteration += 1;
        _nextHopForce /= 2;
    }

    private void EndWallClimb()
    {
        _endClimbTimer.Stop();
        SetPhysicsProcess(false);

        _isWallClimbing = false;
        OnStop?.Invoke(this, EventArgs.Empty);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_ledgeClimb.CanLedgeClimb())
            return;
        
        EndWallClimb();
        
        Vector3 velCoef = new(1, 0, 1);
        _controller.Velocity *= velCoef;
        _controller.RealVelocity *= velCoef;
        _ledgeClimb.DoLedgeClimb();
    }
}