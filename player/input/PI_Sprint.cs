using System;
using Godot;

[GlobalClass]
public partial class PI_Sprint : Node
{
    [Export] public PM_Jump Jump {get; private set;}
    [Export] public PI_Walk WalkInput {get; private set;}
    [Export] public bool Hold {get; private set;} = false;
    public EventHandler OnStartSprinting;
    public EventHandler OnStopSprinting;

    private bool _active = false;       // Isn't called "_isSpriting" as it does not exactly reflect the sprinting state, just the input state

    public override void _Ready()
    {
        Jump.OnJump += (o, f) => StopSprinting();
        WalkInput.OnStopOrBackward += (o, f) => StopSprinting();
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (Hold)
            HandleHold(@event);
        else
            HandleSimple(@event);
    }

    public void Reset() => _active = false;

    private void HandleHold(InputEvent @event)
    {
        if (@event.IsActionPressed("sprint") && !@event.IsEcho())
            StartSprinting();
        else if(@event.IsActionReleased("sprint"))
            StopSprinting();
    }

    private void HandleSimple(InputEvent @event)
    {
        if (@event.IsActionPressed("sprint") && !@event.IsEcho())
        {
            if (_active)
                StopSprinting();
            else
                StartSprinting();
        }
    }

    private void StopSprinting()
    {
        if (_active)
        {
            _active = false;
            OnStopSprinting?.Invoke(this, EventArgs.Empty);
        }
    }

    private void StartSprinting()
    {
        _active = true;
        OnStartSprinting?.Invoke(this, EventArgs.Empty);
    }
}