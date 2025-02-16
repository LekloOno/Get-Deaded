using System;
using Godot;

[GlobalClass]
public partial class PM_Jump : PM_Action
{
    [Export] public PI_Jump JumpProcess {get; private set;}
    [Export] public float JumpVelocity {get; private set;}

    public Vector3 Jump(Vector3 velocity)
    {
        if (GroundState.IsGrounded() && JumpProcess.UseBuffer())
		{
			velocity.Y = JumpVelocity;
            JumpProcess.SetLastJumped();
            GroundState.UpdateGrounded(false);
		}

        return velocity;
    }
}