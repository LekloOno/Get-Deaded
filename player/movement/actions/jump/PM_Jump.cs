using System;
using Godot;

[GlobalClass]
public partial class PM_Jump : PM_Action
{
    [Export] private PI_Jump _jumpInput;
    [Export] private PM_JumpData _data;
    [Export] private PS_Grounded _groundState;

    private float tracker_jumpFatigueRecover;

    public Vector3 Jump(Vector3 velocity)
    {
        if (_groundState.IsGrounded() && _jumpInput.UseBuffer())
            return DoJump(velocity);

        return velocity;
    }

    private Vector3 DoJump(Vector3 velocity)
    {
        float force = ComputeForce();
        velocity.Y = force;
        _jumpInput.SetLastJumped();
        _groundState.UpdateGrounded(false);
        
        OnStart?.Invoke(this, EventArgs.Empty);
        return velocity;
    }

    private float ComputeForce()
    {
        ulong fatigueTime = Time.GetTicksMsec() - _jumpInput.LastJumped;

        if (_data.FatigueMsec == 0)      // Avoid 0 division
            return _data.Force;

        if (_data.FatigueFloorMsec == _data.FatigueMsec)  // Avoid 0 division
        {
            if (fatigueTime >= _data.FatigueMsec)
                return _data.Force;
            return _data.FatigueForce;
        }

        ulong scaledFatigueTime = fatigueTime/_data.FatigueMsec;
        float scaledFloor = (float)_data.FatigueFloorMsec/_data.FatigueFloorMsec;
        
        float unclampedRatio = scaledFloor * scaledFatigueTime - scaledFloor + 1f;
        float ratio = Mathf.Clamp(unclampedRatio, 0, 1);

        return ratio * (_data.Force - _data.FatigueForce) + _data.FatigueForce;
    }
}