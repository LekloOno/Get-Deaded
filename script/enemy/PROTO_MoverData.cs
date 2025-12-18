using Godot;

[GlobalClass]
public partial class PROTO_MoverData : Resource
{
    [ExportCategory("Straffe")]
    [Export] public float MinStraffe {get; private set;} = 0.06f;
    [Export] public float MaxStraffe {get; private set;} = 1.5f;

    [ExportCategory("Base Speed")]
    [Export] public float Acceleration {get; private set;} = 20f;
    [Export] public float SprintSpeed {get; private set;} = 4.2f;
    
    [ExportCategory("Speed variations")]
    [Export] public float RunSpeed {get; private set;} = 3.3f;
    [Export] public float WalkSpeed {get; private set;} = 2.5f;
    [Export] public float WalkWeight {get; private set;} = 0.15f;
    [Export] public float RunWeight {get; private set;} = 0.35f;
    [Export] public float SprintWeight {get; private set;} = 0.50f;
    [Export] public float MinHold {get; private set;} = 0.1f;
    [Export] public float MaxHold {get; private set;} = 3f;

    public float TotalWeight => WalkWeight + RunWeight + SprintWeight;
    public float PropWalk => WalkWeight / TotalWeight;
    public float PropRun => RunWeight / TotalWeight;
    public float PropSprint => SprintWeight / TotalWeight;
}