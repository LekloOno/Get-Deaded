// Gets the crouch input 
// checks global states and dispatch the call to the right input handler
using System;
using Godot;

[GlobalClass]
public partial class PI_CrouchDispatcher : Node
{
    [Export] public PI_Slide SlideInput {get; private set;}
    [Export] public PI_Dash DashInput {get; private set;}
    public EventHandler OnStartCrouching;
    public EventHandler OnStopCrouching;

    private bool _active = false;

    public override void _Ready()
    {
        //Jump.OnJump += (o, f) => StopSprinting();
        //WalkInput.OnStopOrBackward += (o, f) => StopSprinting();
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event.IsActionPressed("crouch") && !@event.IsEcho())
        {
            DashInput.KeyDown();
            SlideInput.KeyDown();
        }
        else if(@event.IsActionReleased("crouch"))
        {
            SlideInput.KeyUp();
        }
    }
}