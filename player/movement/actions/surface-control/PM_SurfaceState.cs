using System;
using Godot;

[GlobalClass]
public partial class PM_SurfaceState : Node
{
    // [Export] public PM_Crouch CrouchHandler {get; private set;}
    [Export] public PM_SurfaceStateData StateData {get; private set;}
    [Export] public PI_Sprint SprintInput {get; private set;}
    [Export] public PM_Crouch Crouch {get; private set;}
    public PM_SurfaceData CurrentData {get; protected set;}

    public override void _Ready()
    {
        CurrentData = StateData.Normal;
        SprintInput.OnStartSprinting += (o, e) => CurrentData = StateData.Sprint;
        SprintInput.OnStopSprinting += (o, e) => CurrentData = StateData.Normal;
        
        Crouch.OnStartCrouch += SetCrouch;
        Crouch.OnStopCrouch += (o, e) => CurrentData = StateData.Normal;
    }

    public void SetCrouch(object sender, EventArgs e)
    {
        CurrentData = StateData.Crouch;
    }
}