using Godot;
using System;

public partial class PM_Controller : CharacterBody3D
{
	[Export] public PI_Walk WalkProcess {get; private set;}
	[Export] public PI_Jump JumpProcess {get; private set;}
	[Export] public PS_Grounded GroundState {get; private set;}
    [Export] public PC_Control CameraControl {get; private set;}
    [Export] public PM_SurfaceControl SurfaceControl {get; private set;}
	
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

    public override void _Ready()
	{
        // UnhandledKeyInput is usually called _before_ UnhandledInput
        // We want to make sure player's rotation is updated before handling movement input as wishDir depends on it.
        // Reduces "input lag" by one frame.
        CameraControl.SetProcessUnhandledInput(false);
        WalkProcess.SetProcessUnhandledKeyInput(false);
        // SurfaceControl has logic inside PhysicsProcess so we can debug it independantly from other nodes.
        // However, we want to control the exact execution flow of it in real context.
        SurfaceControl.SetPhysicsProcess(false);
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

		Vector3 velocity = Velocity;


		// Add the gravity.
		if (!GroundState.IsGrounded())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (GroundState.IsGrounded() && JumpProcess.UseBuffer())
		{
			velocity.Y = JumpVelocity;
		}

		velocity += SurfaceControl.Accelerate(velocity, (float)delta);
        velocity = SurfaceControl.ApplyDrag(velocity, delta);

		Velocity = velocity;
		MoveAndSlide();
	}
}
