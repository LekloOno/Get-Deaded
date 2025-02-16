using Godot;
using System;

public partial class PM_Controller : CharacterBody3D
{
	[Export] public PI_Walk WalkProcess {get; private set;}
	[Export] public PI_Jump JumpProcess {get; private set;}
	[Export] public PS_Grounded GroundState {get; private set;}
    [Export] public PC_Control CameraControl {get; private set;}
	
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

    public override void _Ready()
	{
        CameraControl.SetProcessUnhandledInput(false);
        WalkProcess.SetProcessUnhandledKeyInput(false);
	}

    public override void _UnhandledInput(InputEvent @event)
    {
        // UnhandledKeyInput is usually called _before_ UnhandledInput
        // We want to make sure player's rotation is updated before handling movement input as wishDir depends on it.
        // Reduces "input lag" by one frame.
        CameraControl._UnhandledInput(@event);
        WalkProcess._UnhandledKeyInput(@event);
    }

	public override void _PhysicsProcess(double delta)
	{
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

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector3 direction = WalkProcess.WishDir;
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
