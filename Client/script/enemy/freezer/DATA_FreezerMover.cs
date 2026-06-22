using Godot;

[GlobalClass]
public partial class DATA_FreezerMover : Resource
{
    [Export] public float FocusDistance     { get; private set; }
    [Export] public float Drag              { get; private set; }
    [Export] public float Acceleration      { get; private set; }
    [Export] public float MaxSpeed          { get; private set; }
    [Export] public float MinStraffe        { get; private set; }
    [Export] public float MaxStraffe        { get; private set; }
    [Export] public float FloatAcceleration { get; private set; }
    [Export] public float FloatMaxSpeed     { get; private set; }
    [Export] public float MinFloat          { get; private set; }
    [Export] public float MaxFloat          { get; private set; }
}