using System;
using Godot;

[GlobalClass]
public partial class CNT_SprintWeapon : Node
{
    [Export] private PI_Sprint _sprintInput;
    [Export] private PW_WeaponsHandler _weaponsHandler;
    public override void _Ready()
    {
        _weaponsHandler.ADSStarted += () => _sprintInput.HandleExternal(PI_ActionState.STOPPED, new());
        _sprintInput.Start += CheckSprintStart; 
    }

    private void CheckSprintStart(object sender, EmptyInput args)
    {
        if (_weaponsHandler.ADSactive)
            _sprintInput.HandleExternal(PI_ActionState.STOPPED, new());
    }
}