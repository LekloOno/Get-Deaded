using System;
using Godot;

[GlobalClass]
public partial class CNT_AutoSprint : Node
{
    [Export] private PI_Sprint _sprintInput;
    [Export] private PI_Walk _walkInput;
    [Export] private PI_Crouch _crouchInput;
    [Export] private PI_Slide _slideInput;
    [Export] private PW_WeaponsHandler _weaponsHandler;

    private bool _enabled;

    public override void _Ready()
    {
        Enable();
    }

    private void OnSettingChanged(GodotObject sender, Variant value)
    {
        if ((bool) value)
            Enable();
        else
            Disable();
    }

    private void Disable()
    {
        if (!_enabled)
            return;

        _enabled = true;
        AddChild(_sprintInput);
        _weaponsHandler.Shot -= HandleShot;
        _weaponsHandler.Shot -= GraceShot;

        _weaponsHandler.ADSStopped -= CheckSprint;
        _walkInput.KeyPressed -= OnKeyPressed;
        _crouchInput.OnStopInput -= OnCrouchStopped;
        _slideInput.OnStopInput -= OnCrouchStopped;
    }

    private void OnCrouchStopped(object? sender, EventArgs e) =>
        CheckSprint();

    private void Enable()
    {
        if (_enabled)
            return;

        _enabled = false;
        _sprintInput.GetParent().RemoveChild(_sprintInput);
        
        _weaponsHandler.Shot += HandleShot;
        _weaponsHandler.ADSStopped += CheckSprint;
        _walkInput.KeyPressed += OnKeyPressed;
        _crouchInput.OnStopInput += OnCrouchStopped;
        _slideInput.OnStopInput += OnCrouchStopped;
    }

    private void CheckSprint()
    {
        if (_sprintInput.Active)
            return;

        if (!_walkInput.IsForwarding())
            return;

        StartSprint();
    }

    private void OnKeyPressed(object? sender, KeyPressedArgs e) =>
        CheckSprint();

    private ulong _lastGraceShot = 0;
    [Export] private ulong _graceShotWindow = 1200;
    private void HandleShot()
    {
        StopSprint();
        _weaponsHandler.Shot -= HandleShot;
        _weaponsHandler.Shot += GraceShot;
    }

    private void GraceShot()
    {
        ulong now = PHX_Time.ScaledTicksMsec;
        
        if ((now - _lastGraceShot) >= _graceShotWindow)
        {
            _weaponsHandler.Shot -= GraceShot;
            _weaponsHandler.Shot += HandleShot;
        }

        _lastGraceShot = now;
    }

    private void StartSprint() =>
        _sprintInput.HandleExternal(PI_ActionState.STARTED, new());
    
    private void StopSprint() =>
        _sprintInput.HandleExternal(PI_ActionState.STOPPED, new());
}