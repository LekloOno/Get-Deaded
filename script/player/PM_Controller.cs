using System;
using Godot;

[GlobalClass]
public partial class PM_Controller : CharacterBody3D
{
    [Export] private PH_Manager _healthManager;
    [Export] private PI_Walk _walkProcess;
    [Export] private PM_WallJump _wallJump;
    [Export] private PM_WallClimb _wallClimb;
    [Export] private PS_Grounded _groundState;
    [Export] private PC_Control _cameraControl;
    [Export] private PM_SurfaceControl _surfaceControl;
    [Export] private PM_VelocityCache _velocityCache;
    [Export] private PM_StraffeSnap _straffeSnap;
    [Export] private float _debugDashStrength = 10f;
    
    public EventHandler OnDie;

    public PHX_ForcesCache AdditionalForces {get; private set;} = new PHX_ForcesCache();  // To allow external entities to apply additional forces.
    public PHX_ForcesCache TakeOverForces {get; private set;} = new PHX_ForcesCache();    // To allow external entities to take over the movement behavior.
    public PM_VelocityCache VelocityCache => _velocityCache;
    public Vector3 RealVelocity {get; set;}
    public Vector3 Acceleration {get; private set;}
    private float _specialGravity = 0;
    private bool _hasSpecialGravity = false;

    private EventHandler<double> _onPhysicsProcess;

    public float SpecialGravity
    {
        set
        {
            _specialGravity = value;
            _hasSpecialGravity = true;
        }
    }

    public void ResetGravity()
    {
        _hasSpecialGravity = false;
    }

    public void Revive()
    {
        _healthManager.Init(true);
        _onPhysicsProcess -= DeadBehavior;
        _onPhysicsProcess += NormalBehavior;
    }

    public override void _Ready()
    {
        _healthManager.TopHealthLayer.OnDie += Die;
    }

    private void NormalBehavior(object sender, double delta)
    {
        if (TakeOverForces.IsEmpty())
        {
            Vector3 velocity = _velocityCache.GetVelocity(this, Velocity, _walkProcess.WishDir, _groundState.IsGrounded(), delta);


            velocity = _wallClimb.WallClimb(velocity);
            velocity = _surfaceControl.ApplyDrag(velocity, delta);

            Vector3 prevVelocity = velocity;
            velocity += _surfaceControl.Accelerate(velocity, (float)delta);
            
            velocity += AdditionalForces.Consume();
            velocity = _straffeSnap.Snap(velocity, prevVelocity);

            if (!_groundState.IsGrounded())
            {
                if (velocity.Y <= 0) ResetGravity();
                Vector3 gravity = _hasSpecialGravity ? (_specialGravity * Vector3.Up) : GetGravity();
                velocity += gravity * (float)delta;
            }

            Velocity = velocity;
        } else 
        {
            Velocity = TakeOverForces.Consume();
        }
    }

    private void Die(GC_Health sender)
    {
        ResetGravity();
        _onPhysicsProcess -= NormalBehavior;
        _onPhysicsProcess += DeadBehavior;
        OnDie?.Invoke(this, EventArgs.Empty);
    }

    private void DeadBehavior(object sender, double delta)
    {
        Vector3 velocity = RealVelocity;
        velocity = _surfaceControl.ApplyDrag(velocity, delta);

        if (!_groundState.IsGrounded())
        {
            Vector3 gravity = _hasSpecialGravity ? (_specialGravity * Vector3.Up) : GetGravity();
            velocity += gravity * (float)delta;
        }

        Velocity = velocity;
    }

    public override void _PhysicsProcess(double delta)
    {
        Acceleration = Vector3.Zero;

        Vector3 startVelocity = RealVelocity;

        Vector3 pos = GlobalPosition;
        _onPhysicsProcess?.Invoke(this, delta);

        MoveAndSlide();
        RealVelocity = (GlobalPosition - pos)/(float)delta;
        Acceleration = (RealVelocity - startVelocity)/(float)delta;
    }
}
