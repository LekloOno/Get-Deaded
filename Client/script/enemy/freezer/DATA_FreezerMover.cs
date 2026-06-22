using Godot;

[GlobalClass]
public partial class DATA_FreezerMover : Resource
{
    [Export] public float FocusMinDistance         { get; private set; }
    [Export] public float FocusMaxDistance         { get; private set; }
    [Export] public float FollowAcceleration    { get; private set; }
    [Export] public float FollowMaxSpeed        { get; private set; }
    [Export] public float Drag                  { get; private set; }
    [Export] public float Acceleration          { get; private set; }
    [Export] public float MaxSpeed              { get; private set; }
    [Export] public float MinStraffe            { get; private set; }
    [Export] public float MaxStraffe            { get; private set; }
    [Export] public float FloatAcceleration     { get; private set; }
    [Export] public float FloatMaxSpeed         { get; private set; }
    [Export] public float MinFloat              { get; private set; }
    [Export] public float MaxFloat              { get; private set; }
}