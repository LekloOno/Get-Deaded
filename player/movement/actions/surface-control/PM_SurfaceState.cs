using System;
using Godot;

[GlobalClass]
public partial class PM_SurfaceState : Node
{
    [Export] private PM_SurfaceStateData _stateData;
    [Export] private PI_Sprint _sprintInput;
    [Export] private PM_Crouch _crouch;
    [Export] private PM_Slide _slide;
    private PM_SurfaceData _currentData;
    public PM_SurfaceData CurrentData => _currentData;

    public override void _Ready()
    {
        _currentData = _stateData.Normal;
        _sprintInput.OnStartSprinting += (o, e) => _currentData = _stateData.Sprint;
        _sprintInput.OnStopSprinting += (o, e) => _currentData = _stateData.Normal;
        
        _crouch.OnStart += SetCrouch;
        _crouch.OnStop += (o, e) => _currentData = _stateData.Normal;

        _slide.OnStart += (o, e) => _currentData = _stateData.Slide;
        _slide.OnStop += (o, e) => _currentData = _stateData.Normal;
    }

    public void SetCrouch(object sender, EventArgs e) => _currentData = _stateData.Crouch;
}