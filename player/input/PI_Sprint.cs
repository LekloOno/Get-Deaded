using System;
using Godot;

[GlobalClass]
public partial class PI_Sprint : Node
{
    [ExportCategory("User Settings")]
    [Export] public bool Hold = false;

    [ExportCategory("Setup")]
    [Export] private PM_Jump _jump;
    [Export] private PI_Walk _walkInput;
    [Export] private PI_CrouchDispatcher _crouchDispatcher;

    public EventHandler OnStartSprinting;
    public EventHandler OnStopSprinting;

    private bool _active = false;       // Isn't called "_isSpriting" as it does not exactly reflect the sprinting state, just the input state

    public override void _Ready()
    {
        _jump.OnStart += (o, f) => StopSprinting();
        _walkInput.OnStopOrBackward += (o, f) => StopSprinting();
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
        if (@event.IsActionPressed("sprint"))
            StartSprinting();
        else if(@event.IsActionReleased("sprint"))
            StopSprinting();
    }

    private void HandleSimple(InputEvent @event)
    {
        if (@event.IsActionPressed("sprint"))
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
        if (!_crouchDispatcher.IsCrouched)
        {
            _active = true;
            OnStartSprinting?.Invoke(this, EventArgs.Empty);
        }
    }
}