using Godot;
using System;

public partial class PM_Controller : CharacterBody3D
{
	[Export] public PI_Walk WalkProcess {get; private set;}
	[Export] public PM_Jump Jump {get; private set;}
	[Export] public PS_Grounded GroundState {get; private set;}
    [Export] public PC_Control CameraControl {get; private set;}
    [Export] public PM_SurfaceControl SurfaceControl {get; private set;}
    [Export] public PM_VelocityCache VelocityCache {get; private set;}
    [Export] public float DashStrength = 10f;

    public Vector3 RealVelocity {get; private set;}
	
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
		
        Vector3 velocity = VelocityCache.IsCached() ? VelocityCache.UseCache() : Velocity;

        if (Input.IsActionJustPressed("click"))
        {
            velocity += CameraControl.GlobalBasis.Z * -DashStrength;
        }

		// Add the gravity.
		if (!GroundState.IsGrounded())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		velocity = Jump.Jump(velocity);

		velocity += SurfaceControl.Accelerate(velocity, (float)delta);
        GD.Print(SurfaceControl.Accelerate(velocity, (float)delta));
        velocity = SurfaceControl.ApplyDrag(velocity, delta);

		Velocity = velocity;

        KinematicCollision3D collision = MoveAndCollide(velocity * (float)delta, true, SafeMargin, true);
        if (collision?.GetCollisionCount() > 0)
        {
            if(collision.GetAngle(0, UpDirection) > FloorMaxAngle)
            {
                VelocityCache.Cache(velocity);
            }
        }
        MoveAndSlide();

        Velocity = (GlobalPosition - pos)/(float)delta;
        //GD.Print((GlobalPosition - pos)/(float)delta, Velocity);
	}
}
