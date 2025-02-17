
using Godot;

[GlobalClass]
public partial class PM_JumpData : Resource
{
    [Export] public float Force {get; private set;}
    [Export] public float FatigueForce {get; private set;}          //The minimum multplier of the jump strength.
    [Export] public uint FatigueMsec {get; private set;}       //The time (msec) required between two jump to get full jump strength.
    [Export] public uint FatigueFloorMsec {get; private set;}  //The time (msec) between which the applied multiplier will be FatigueFloor.
}