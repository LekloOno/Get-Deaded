using System;
using Godot;

[GlobalClass]
public partial class CNT_SprintWeapon : Node
{
    [Export] private PI_Sprint _sprintInput;
    [Export] private PW_WeaponsHandler _weaponsHandler;
    private bool _buffered = false;
    private bool _enabled;
    public override void _Ready()
    {
        Enable();
    }

    public void Enable()
    {
        if (_enabled)
            return;

        _enabled = true;
        _weaponsHandler.ADSStarted += StopSprint;
        _weaponsHandler.ADSStopped += CheckBuffer;
        _sprintInput.Start += CheckSprintStart;
    }

    public void Disable()
    {
        if (!_enabled)
            return;

        _enabled = false;
        _weaponsHandler.ADSStarted -= StopSprint;
        _weaponsHandler.ADSStopped -= CheckBuffer;
        _sprintInput.Start -= CheckSprintStart; 
    }

    private void StopSprint()
    {
        _sprintInput.HandleExternal(PI_ActionState.STOPPED, new());
    }

    private void CheckSprintStart(object sender, EmptyInput args)
    {
        if (!_weaponsHandler.ADSactive)
            return;
        
        StopSprint();
        _buffered = !_buffered;
    }

    private void CheckBuffer()
    {
        if (!_buffered)
            return;

        _buffered = false;
        _sprintInput.HandleExternal(PI_ActionState.STARTED, new());
    }
}