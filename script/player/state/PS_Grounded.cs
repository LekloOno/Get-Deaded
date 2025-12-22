using Godot;
using System;

namespace Pew;

[GlobalClass]
public partial class PS_Grounded : Node
{
    [ExportCategory("Settings")]
    [Export] private float _maxVerticalSpeed = 4.76f;
    
    [ExportCategory("Setup")]
    [Export] private PM_Controller _characterBody3D;
    [Export] private ShapeCast3D _groundCast;

    public event EventHandler<LandingEventArgs> OnLanding;
    public event EventHandler OnLeaving;

    public float DistanceToGround { get; private set; } // Capped to abs(_groundCast.TargetPosition.Y)

    private ulong _lastGrounded = 0; // Only relevant when airborne. Won't be updated while actively grounded.

    private bool _prevGrounded = false;
    private float _prevDownardSpeed;

    
    public override void _Ready()
    {
        SetProcessPriority(-1);
    }
    public override void _PhysicsProcess(double delta)
    {
        if (_groundCast.IsColliding())
        {
            Vector3 collisionPoint = _groundCast.GetCollisionPoint(0);
            DistanceToGround = _groundCast.GlobalPosition.DistanceTo(collisionPoint);
        }
        else
            DistanceToGround = Math.Abs(_groundCast.TargetPosition.Y);
        
        UpdateGrounded();
    }
    public override void _Process(double delta)
    {
        if (_characterBody3D.Velocity.Y < _prevDownardSpeed)
            _prevDownardSpeed = _characterBody3D.Velocity.Y;

        
    }

    public void UpdateGrounded(bool grounded)
    {
        bool realNextGrounded = grounded;
        if(realNextGrounded != _prevGrounded)
        {
            _prevGrounded = realNextGrounded;
            if(realNextGrounded)
            {
                OnLanding?.Invoke(this, new LandingEventArgs(_prevDownardSpeed));
                _prevDownardSpeed = _characterBody3D.Velocity.Y;
            }
            else
            {
                OnLeaving?.Invoke(this, EventArgs.Empty);
                _lastGrounded = PHX_Time.ScaledTicksMsec;
            }
        }
    }

    private void UpdateGrounded() {
        UpdateGrounded(RealGrounded());
    }

    public float MsecSinceLastGrounded() {
        return PHX_Time.ScaledTicksMsec - _lastGrounded;
    }

    public bool IsGrounded() {
        return _prevGrounded;
    }

    private bool RealGrounded() {
        return _characterBody3D.IsOnFloor()
            && Mathf.Abs(_characterBody3D.RealVelocity.Y) < _maxVerticalSpeed;
    }

}