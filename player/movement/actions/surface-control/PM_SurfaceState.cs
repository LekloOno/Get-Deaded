using System;
using Godot;

[GlobalClass]
public partial class PM_SurfaceState : Node
{
    // [Export] public PM_Crouch CrouchHandler {get; private set;}
    // [Export] public PM_Sprint SprintHandler {get; private set;}
    [Export] public PM_SurfaceStateData StateData {get; private set;}
    [Export] public PI_Sprint SprintInput {get; private set;}
    public PM_SurfaceData CurrentData {get; private set;}

    public override void _Ready()
    {
        CurrentData = StateData.Normal;
        SprintInput.OnStartSprinting += (Object o, EventArgs e) => CurrentData = StateData.Sprint;
        SprintInput.OnStopSprinting += (Object o, EventArgs e) => CurrentData = StateData.Normal;
    }
}