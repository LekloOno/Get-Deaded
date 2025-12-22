using System;
using Godot;

namespace Pew;

[GlobalClass]
public partial class PI_Dash : PI_InputKeyAction
{
    public EventHandler OnStartInput {get; set;}   // Called when slide is initiated

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event.IsActionPressed("sprint"))
            KeyDown();
    }

    public void KeyDown() => OnStartInput?.Invoke(this, EventArgs.Empty);
}