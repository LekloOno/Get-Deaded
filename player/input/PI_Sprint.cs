using System;
using Godot;

[GlobalClass]
public partial class PI_Sprint : Node
{
    [Export] public bool Hold {get; private set;} = false;
    public EventHandler OnStartSprinting;
    public EventHandler OnStopSprinting;

    private bool _active = false;       // Isn't called "_isSpriting" as it does not exactly reflect the sprinting state, just the input state

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (Hold)
            HandleHold(@event);
        else
            HandleSimple(@event);
    }

    private void HandleHold(InputEvent @event)
    {
        if (@event.IsActionPressed("sprint") && !@event.IsEcho())
        {
            _active = true;
            OnStartSprinting?.Invoke(this, EventArgs.Empty);
        }
        else if(@event.IsActionReleased("sprint"))
        {
            _active = false;
            OnStopSprinting?.Invoke(this, EventArgs.Empty);
        }
    }

    private void HandleSimple(InputEvent @event)
    {
        if (@event.IsActionPressed("sprint") && !@event.IsEcho())
        {
            if (_active)
            {
                _active = false;
                OnStopSprinting?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                _active = true;
                OnStartSprinting?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}