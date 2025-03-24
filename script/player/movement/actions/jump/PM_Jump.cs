using System;
using Godot;

[GlobalClass]
public partial class PM_Jump : PM_Action
{
    [Export] private PI_Jump _jumpInput;
    [Export] private PM_JumpData _data;
    [Export] private PS_Grounded _groundState;
    [Export] private PM_Controller _controller;
    
    private float _jumpGravity;
    private float _fallGravity;
    private float _jumpVelocity;
    private float _fatigueJumpGravity;
    private float _fatigueFallGravity;
    private float _fatigueJumpVelocity;

    private float tracker_jumpFatigueRecover;

    public override void _Ready()
    {
        _jumpGravity = 2f*_data.JumpHeight/Mathf.Pow(_data.JumpPeakTime, 2f);
        _fallGravity = 2f*_data.JumpHeight/Mathf.Pow(_data.JumpFallTime, 2f);
        _jumpVelocity = _jumpGravity * _data.JumpPeakTime;

        _fatigueJumpGravity = 2f*_data.FatigueJumpHeight/Mathf.Pow(_data.FatigueJumpPeakTime, 2f);
        _fallGravity = 2f*_data.FatigueJumpHeight/Mathf.Pow(_data.FatigueJumpFallTime, 2f);
        _fatigueJumpVelocity = _fatigueJumpGravity * _data.FatigueJumpPeakTime;
    }

    public Vector3 Jump(Vector3 velocity)
    {
        if (_groundState.IsGrounded() && _jumpInput.UseBuffer())
            return DoJump(velocity);

        return velocity;
    }

    private Vector3 DoJump(Vector3 velocity)
    {
        (float, float, float) forces = ComputeForce();
        velocity.Y = forces.Item1;
        _controller.SpecialGravity = -forces.Item2;

        _jumpInput.SetLastJumped();
        _groundState.UpdateGrounded(false);
        
        OnStart?.Invoke(this, EventArgs.Empty);
        return velocity;
    }

    private (float, float, float) ComputeForce()
    {
        ulong fatigueTime = Time.GetTicksMsec() - _jumpInput.LastJumped;

        if (_data.FatigueMsec == 0)      // Avoid 0 division
            //return _data.Force;
            return (_jumpVelocity, _jumpGravity, _fallGravity);;

        if (_data.FatigueFloorMsec == _data.FatigueMsec)  // Avoid 0 division
        {
            if (fatigueTime >= _data.FatigueMsec)
                //return _data.Force;
                return (_jumpVelocity, _jumpGravity, _fallGravity);
            //return _data.FatigueForce;
            return (_fatigueJumpVelocity, _fatigueJumpGravity, _fatigueFallGravity);
        }

        return fatigueTime > _data.FatigueMsec ? (_jumpVelocity, _jumpGravity, _fallGravity) : (_fatigueJumpVelocity, _fatigueJumpGravity, _fatigueFallGravity);
/*
        ulong scaledFatigueTime = fatigueTime/_data.FatigueMsec;
        float scaledFloor = (float)_data.FatigueFloorMsec/_data.FatigueFloorMsec;
        
        float unclampedRatio = scaledFloor * scaledFatigueTime - scaledFloor + 1f;
        float ratio = Mathf.Clamp(unclampedRatio, 0, 1);

        return ratio * (_data.Force - _data.FatigueForce) + _data.FatigueForce;*/
    }
}