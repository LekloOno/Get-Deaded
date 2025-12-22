using Godot;

namespace Pew;

[GlobalClass]
public partial class PM_JumpData : Resource
{
    [Export] public float Force {get; private set;}
    [Export] public float FatigueForce {get; private set;}          //The minimum multplier of the jump strength.
    [Export] public uint FatigueMsec {get; private set;}       //The time (msec) required between two jump to get full jump strength.
    [Export] public uint FatigueFloorMsec {get; private set;}  //The time (msec) between which the applied multiplier will be FatigueFloor.

    [Export] public float JumpHeight {get; private set;} = 1.1f;
    [Export] public float JumpPeakTime {get; private set;} = 0.5f;
    [Export] public float JumpFallTime {get; private set;} = 0.5f;
    [Export] public float FatigueJumpHeight {get; private set;} = 0.4f;
    [Export] public float FatigueJumpPeakTime {get; private set;} = 0.3f;
    [Export] public float FatigueJumpFallTime {get; private set;} = 0.3f;
}