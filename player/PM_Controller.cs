using Godot;
using System;
using System.Linq;

public partial class PM_Controller : CharacterBody3D
{
	[Export] public PI_Walk WalkProcess {get; private set;}
	[Export] public PM_Jump Jump {get; private set;}
	[Export] public PS_Grounded GroundState {get; private set;}
    [Export] public PC_Control CameraControl {get; private set;}
    [Export] public PM_SurfaceControl SurfaceControl {get; private set;}
    [Export] public PM_VelocityCache VelocityCache {get; private set;}
    [Export] public PM_StraffeSnap StraffeSnap {get; private set;}
    [Export] public float DashStrength = 10f;
    //[Export] public PM_StepClimb StepClimb {get; private set;}

    public PH_ForcesCache AdditionalForces {get; private set;} = new PH_ForcesCache();  // To allow external entities to apply additional forces.
    public PH_ForcesCache TakeOverForces {get; private set;} = new PH_ForcesCache();    // To allow external entities to take over the movement behavior.
    public Vector3 RealVelocity {get; set;}
	
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

    public override void _Ready()
	{
        // UnhandledKeyInput is usually called _before_ UnhandledInput
        // We want to make sure player's rotation is updated before handling movement input as wishDir depends on it.
        // Reduces "input lag" by one frame.
        CameraControl.SetProcessUnhandledInput(false);
        WalkProcess.SetProcessUnhandledKeyInput(false);
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        CameraControl._UnhandledInput(@event);
        WalkProcess._UnhandledKeyInput(@event);
    }

	public override void _PhysicsProcess(double delta)
	{
        // Grounded, state is updated before automatically, it has a set priority

        // Handle Jump
        
        // Handle Slide

        // Handle Friction

        // Handle Surface Control

        // Collide and slide OR Step climber

        Vector3 pos = GlobalPosition;

        
		
        //Vector3 velocity = VelocityCache.IsCached() ? VelocityCache.UseCache() : Velocity;

		// Add the gravity.
		

		// Handle Jump.
        if (TakeOverForces.IsEmpty())
        {
            Vector3 velocity = GroundState.IsGrounded() ? Velocity : RealVelocity;
            //Vector3 velocity = Velocity;
            velocity = VelocityCache.GetVelocity(this, velocity, Velocity, delta);

            Vector3 prevVelocity = velocity;
            
            if (Input.IsActionJustPressed("click"))
            {
                velocity += CameraControl.GlobalBasis.Z * -DashStrength;
            }


            velocity = Jump.Jump(velocity);
            velocity = SurfaceControl.ApplyDrag(velocity, delta);
            velocity += SurfaceControl.Accelerate(velocity, (float)delta);
            velocity += AdditionalForces.Consume();
            velocity = StraffeSnap.Snap(velocity, prevVelocity);

            if (!GroundState.IsGrounded())
                velocity += GetGravity() * (float)delta;

            Velocity = velocity;
        } else 
        {
            Velocity = TakeOverForces.Consume();
        }

    /*
        if (!StepClimb.Climb(this, velocity, WalkProcess.WishDir, VelocityCache, GetWorld3D().DirectSpaceState, delta))
        {
            if (!GroundState.IsGrounded())
            {
                velocity += GetGravity() * (float)delta;
            }

            Velocity = velocity;
            MoveAndSlide();
        }*/

        MoveAndSlide();
        RealVelocity = (GlobalPosition - pos)/(float)delta;

        
/*
        if (IsOnWall())
        {
            if(!VelocityCache.IsCached()) VelocityCache.Cache(velocity);
            
            if(velocity.Y == 0)
            {
                Velocity = new Vector3(RealVelocity.X, 0, RealVelocity.Z);
            }
            else
            {
                Velocity = RealVelocity;
            }
        }*/

        //velocity = StepClimb.Climb(this, velocity, WalkProcess.WishDir, VelocityCache, GetWorld3D().DirectSpaceState, delta);
        //velocity = StepClimb.ProcessClimbAndSlides(this, velocity, WalkProcess.WishDir, VelocityCache, GetWorld3D().DirectSpaceState, delta);
        //Velocity = velocity;
        //MoveAndSlide();

        
	}
}
