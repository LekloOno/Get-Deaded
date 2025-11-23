using System;
using Godot;

[GlobalClass]
public partial class PI_Weapons : PI_InputGlobalAction
{
    [Export] private PI_Reload _reloadInput;

    public bool ReloadIsBuffered() => _reloadInput.IsBuffered();
    public bool ReloadUseBuffer() => _reloadInput.UseBuffer();

    public EventHandler OnStartPrimary;
    public EventHandler OnStartSecondary;
    public EventHandler OnStopPrimary;
    public EventHandler OnStopSecondary;
    public EventHandler OnSwitch;
    public EventHandler OnHolster;
    public EventHandler OnReload;
    public EventHandler OnStartMelee;
    public EventHandler OnStopMelee;

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
        else if (@event.IsActionPressed(ACTIONS_Combat.HOLSTER))
            OnHolster?.Invoke(this, EventArgs.Empty);
        else if (@event.IsActionPressed(ACTIONS_Combat.MELEE))
            OnStartMelee?.Invoke(this, EventArgs.Empty);
        else if (@event.IsActionReleased(ACTIONS_Combat.MELEE))
            OnStopMelee?.Invoke(this, EventArgs.Empty);
    }

    private void ForwardReload(object sender, EmptyInput e) => OnReload?.Invoke(this, EventArgs.Empty);

    public override void EnableAction()
    {
        base.EnableAction();
        _reloadInput.EnableAction();
        _reloadInput.Start += ForwardReload;
    }
}