using System;
using Godot;

[GlobalClass]
public partial class PI_Weapons : PI_InputGlobalAction
{
    public EventHandler OnStartPrimary;
    public EventHandler OnStartSecondary;
    public EventHandler OnStopPrimary;
    public EventHandler OnStopSecondary;
    public EventHandler OnSwitch;
    public EventHandler OnHolster;
    public EventHandler OnReload;

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed(ACTIONS_Combat.PRIMARY))
            OnStartPrimary?.Invoke(this, EventArgs.Empty);
        else if (@event.IsActionPressed(ACTIONS_Combat.SECONDARY))
            OnStartSecondary?.Invoke(this, EventArgs.Empty);
        else if (@event.IsActionReleased(ACTIONS_Combat.PRIMARY))
            OnStopPrimary?.Invoke(this, EventArgs.Empty);
        else if (@event.IsActionReleased(ACTIONS_Combat.SECONDARY))
            OnStopSecondary?.Invoke(this, EventArgs.Empty);
        else if (@event.IsActionPressed(ACTIONS_Combat.SWITCH))
            OnSwitch?.Invoke(this, EventArgs.Empty);
        else if (@event.IsActionReleased(ACTIONS_Combat.HOLSTER))
            OnHolster?.Invoke(this, EventArgs.Empty);
        else if (@event.IsActionPressed(ACTIONS_Combat.RELOAD))
            OnReload?.Invoke(this, EventArgs.Empty);
    }
}