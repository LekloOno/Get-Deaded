using System;
using Godot;

[GlobalClass]
public partial class PI_Crouch : Node, PI_CrouchDerived
{
    [Export] private PI_Sprint _sprintInput;
    public PI_Sprint SprintInput => _sprintInput;

    public EventHandler OnStartInput {get; set;}
    public EventHandler OnStopInput {get; set;}

    public bool IsActive {get; set;} = false;

    public void InputStart() => StartCrouching();
    public void InputStop() => StopCrouching();

    public void Reset()
    {
        IsActive = false;
    }

    private void StopCrouching()
    {
        ((PI_CrouchDerived)this).StopAction();
    }

    public void StartCrouching()
    {
        ((PI_CrouchDerived)this).StartAction();
    }
}