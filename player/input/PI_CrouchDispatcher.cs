// Gets the crouch input 
// checks global states and dispatch the call to the right input handler
using System;
using Godot;

[GlobalClass]
public partial class PI_CrouchDispatcher : Node
{
    [Export] private PI_Slide _slideInput;
    [Export] private PI_Dash _dashInput;

    public ulong LastCrouchDown {get; private set;} = 0; 
    private bool _active = false;

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event.IsActionPressed("crouch"))
        {
            LastCrouchDown = Time.GetTicksMsec();
            _dashInput.KeyDown();
            _slideInput.KeyDown();
        }
        else if(@event.IsActionReleased("crouch"))
        {
            _slideInput.KeyUp();
        }
    }
}