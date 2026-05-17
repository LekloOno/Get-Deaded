using System;
using Godot;

[GlobalClass]
public partial class CNT_SprintWeapon : Node
{
    [Export] private PI_Sprint _sprintInput;
    [Export] private PW_WeaponsHandler _weaponsHandler;
    private bool _buffered = false;
    public override void _Ready()
    {
        _weaponsHandler.ADSStarted += () => _sprintInput.HandleExternal(PI_ActionState.STOPPED, new());
        _weaponsHandler.ADSStopped += CheckBuffer;
        _sprintInput.Start += CheckSprintStart;
    }

    private void CheckSprintStart(object sender, EmptyInput args)
    {
        if (!_weaponsHandler.ADSactive)
            return;
        
        _sprintInput.HandleExternal(PI_ActionState.STOPPED, new());
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