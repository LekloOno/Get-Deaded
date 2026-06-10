
using Godot;

[GlobalClass]
public partial class PM_JumpData : Resource
{
    [Export] public float Force {get; private set;} = 4f;
    [Export] public float FatigueForce {get; private set;} = 2f;          //The minimum multplier of the jump strength.
    [Export] public ulong FatigueMsec {get; private set;} = 1200;       //The time (msec) required between two jump to get full jump strength.
    [Export] public ulong FatigueFloorMsec {get; private set;} = 950;  //The time (msec) between which the applied multiplier will be FatigueFloor.
    [Export] public ulong CoyoteTime {get; private set;} = 150;

    [Export] public float JumpHeight {get; private set;} = 1.2f;
    [Export] public float JumpPeakTime {get; private set;} = 0.38f;
    [Export] public float JumpFallTime {get; private set;} = 0.5f;
    [Export] public float FatigueJumpHeight {get; private set;} = 0.35f;
    [Export] public float FatigueJumpPeakTime {get; private set;} = 0.18f;
    [Export] public float FatigueJumpFallTime {get; private set;} = 0.3f;
}