using System;
using Godot;

[GlobalClass]
public partial class PI_Dash : Node
{
    public EventHandler OnStartInput {get; set;}   // Called when slide is initiated

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event.IsActionPressed("sprint"))
            KeyDown();
    }

    public void KeyDown() => OnStartInput?.Invoke(this, EventArgs.Empty);
}