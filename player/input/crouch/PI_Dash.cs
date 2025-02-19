using System;
using Godot;

[GlobalClass]
public partial class PI_Dash : Node
{
    public EventHandler OnStartInput {get; set;}   // Called when slide is initiated

    public void KeyDown() => OnStartInput?.Invoke(this, EventArgs.Empty);
}