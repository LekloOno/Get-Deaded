using Godot;

public partial class PM_Controller : CharacterBody3D
{
    [Export] private PI_Walk _walkProcess;
    [Export] private PM_WallJump _wallJump;
    [Export] private PM_WallClimb _wallClimb;
    [Export] private PS_Grounded _groundState;
    [Export] private PC_Control _cameraControl;
    [Export] private PM_SurfaceControl _surfaceControl;
    [Export] private PM_VelocityCache _velocityCache;
    [Export] private PM_StraffeSnap _straffeSnap;
    [Export] private float _debugDashStrength = 10f;

    public PHX_ForcesCache AdditionalForces {get; private set;} = new PHX_ForcesCache();  // To allow external entities to apply additional forces.
    public PHX_ForcesCache TakeOverForces {get; private set;} = new PHX_ForcesCache();    // To allow external entities to take over the movement behavior.
    public PM_VelocityCache VelocityCache => _velocityCache;
    public Vector3 RealVelocity {get; set;}
    public Vector3 Acceleration {get; private set;}

    public override void _Ready()
    {
        // UnhandledKeyInput is usually called _before_ UnhandledInput
        // We want to make sure player's rotation is updated before handling movement input as wishDir depends on it.
        // Reduces "input lag" by one frame.
        _cameraControl.SetProcessUnhandledInput(false);
        _walkProcess.SetProcessUnhandledKeyInput(false);
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        _cameraControl._UnhandledInput(@event);
        _walkProcess._UnhandledKeyInput(@event);
    }

    public override void _PhysicsProcess(double delta)
    {
        // Grounded, state is updated before automatically, it has a set priority

        // Handle Jump
        
        // Handle Slide

        // Handle Friction

        // Handle Surface Control

        // Collide and slide OR Step climber
        Acceleration = Vector3.Zero;

        Vector3 startVelocity = Velocity;

        Vector3 pos = GlobalPosition;
        if (TakeOverForces.IsEmpty())
        {
            Vector3 velocity = _velocityCache.GetVelocity(this, Velocity, _walkProcess.WishDir, _groundState.IsGrounded(), delta);

            Vector3 prevVelocity = velocity;
            
            if (Input.IsActionJustPressed("click"))
            {
                velocity += _cameraControl.GlobalBasis.Z * -_debugDashStrength;
            }


            velocity = _wallClimb.WallClimb(velocity);
            velocity = _surfaceControl.ApplyDrag(velocity, delta);
            velocity += _surfaceControl.Accelerate(velocity, (float)delta);
            velocity += AdditionalForces.Consume();
            velocity = _straffeSnap.Snap(velocity, prevVelocity);

            if (!_groundState.IsGrounded())
                velocity += GetGravity() * (float)delta;

            Velocity = velocity;
        } else 
        {
            Velocity = TakeOverForces.Consume();
        }

        Acceleration = (Velocity - startVelocity)/(float)delta;
        MoveAndSlide();
        RealVelocity = (GlobalPosition - pos)/(float)delta;
    }
}
