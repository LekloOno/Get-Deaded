using Godot;

[GlobalClass]
public partial class PM_Jump : PM_Action
{
    [Export] public PI_Jump JumpProcess {get; private set;}
    [Export] public PM_JumpData Data {get; private set;}

    private float tracker_jumpFatigueRecover;

    public Vector3 Jump(Vector3 velocity)
    {
        if (GroundState.IsGrounded() && JumpProcess.UseBuffer())
			return DoJump(velocity);

        return velocity;
    }

    private Vector3 DoJump(Vector3 velocity)
    {
        velocity.Y = ComputeForce();
        JumpProcess.SetLastJumped();
        GroundState.UpdateGrounded(false);

        return velocity;
    }

    private float ComputeForce()
    {
        ulong fatigueTime = Time.GetTicksMsec() - JumpProcess.LastJumped;

        if (Data.FatigueMsec == 0)      // Avoid 0 division
            return Data.Force;

        if (Data.FatigueFloorMsec == Data.FatigueMsec)  // Avoid 0 division
        {
            if (fatigueTime >= Data.FatigueMsec)
                return Data.Force;
            return Data.FatigueForce;
        }

        ulong scaledFatigueTime = fatigueTime/Data.FatigueMsec;
        float scaledFloor = (float)Data.FatigueFloorMsec/Data.FatigueFloorMsec;
        
        float unclampedRatio = scaledFloor * scaledFatigueTime - scaledFloor + 1f;
        float ratio = Mathf.Clamp(unclampedRatio, 0, 1);

        return ratio * (Data.Force - Data.FatigueForce) + Data.FatigueForce;
    }
}