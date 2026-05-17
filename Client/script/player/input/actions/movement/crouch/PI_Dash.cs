using System;
using Godot;

[GlobalClass]
public partial class PI_Dash : PI_InputGlobalAction
{
    public EventHandler OnStartInput {get; set;}   // Called when slide is initiated

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("dash"))
            KeyDown();
    }

    public void KeyDown() => OnStartInput?.Invoke(this, EventArgs.Empty);
}