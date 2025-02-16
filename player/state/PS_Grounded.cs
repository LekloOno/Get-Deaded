using Godot;
using System;

[GlobalClass]
public partial class PS_Grounded : Node
{
    [Export] public CharacterBody3D CharacterBody3D {get; private set;}
    [Export] public ShapeCast3D GroundCast {get; private set;}
    [Export] public float MaxVerticalSpeed {get; private set;} = 4.76f;

    public event EventHandler<LandingEventArgs> OnLanding;
    public event EventHandler OnLeaving;

    public float DistanceToGround { get; private set; } // Capped to abs(GroundCast.TargetPosition.Y)

    private ulong _lastGrounded = 0; // Only relevant when airborne. Won't be updated while actively grounded.

    private bool _prevGrounded = false;
    private float _prevDownardSpeed;

    
    public override void _Ready()
    {
        SetProcessPriority(-1);
    }
    public override void _PhysicsProcess(double delta)
    {
        if (GroundCast.IsColliding())
        {
            Vector3 collisionPoint = GroundCast.GetCollisionPoint(0);
            DistanceToGround = GroundCast.GlobalPosition.DistanceTo(collisionPoint);
        }
        else
            DistanceToGround = Math.Abs(GroundCast.TargetPosition.Y);
        
        UpdateGrounded();
    }
    public override void _Process(double delta)
    {
        if (CharacterBody3D.Velocity.Y < _prevDownardSpeed)
            _prevDownardSpeed = CharacterBody3D.Velocity.Y;

        
    }

    private void UpdateGrounded() {
        bool realNextGrounded = RealGrounded();
        if(realNextGrounded != _prevGrounded)
        {
            _prevGrounded = realNextGrounded;
            if(realNextGrounded)
            {
                OnLanding?.Invoke(this, new LandingEventArgs(_prevDownardSpeed));
                _prevDownardSpeed = CharacterBody3D.Velocity.Y;
            }
            else
            {
                OnLeaving?.Invoke(this, EventArgs.Empty);
                _lastGrounded = Time.GetTicksMsec();
            }
        }
    }

    public float MsecSinceLastGrounded() {
        return Time.GetTicksMsec() - _lastGrounded;
    }

    public bool IsGrounded() {
        return _prevGrounded;
    }

    private bool RealGrounded() {
        return CharacterBody3D.IsOnFloor()
            && Mathf.Abs(CharacterBody3D.Velocity.Y) < MaxVerticalSpeed;
    }

}