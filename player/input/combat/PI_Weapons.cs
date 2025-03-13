using System;
using Godot;

[GlobalClass]
public partial class PI_Weapons : Node
{
    public EventHandler OnStartPrimary;
    public EventHandler OnStartSecondary;
    public EventHandler OnStopPrimary;
    public EventHandler OnStopSecondary;

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("primary"))
            OnStartPrimary?.Invoke(this, EventArgs.Empty);
        else if (@event.IsActionPressed("secondary"))
            OnStartSecondary?.Invoke(this, EventArgs.Empty);
        else if (@event.IsActionReleased("primary"))
            OnStopPrimary?.Invoke(this, EventArgs.Empty);
        else if (@event.IsActionReleased("secondary"))
            OnStopSecondary?.Invoke(this, EventArgs.Empty);
    }
}