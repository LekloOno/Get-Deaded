using System;
using Godot;

namespace Pew;

[GlobalClass]
public partial class PC_SprintLean : Node3D
{
    
    [ExportCategory("Setup")]
    [Export(PropertyHint.Range, "-0.5,0,0.01")]
    private float _heightDisplacement = -0.2f;
    
    [Export(PropertyHint.Range, "0.2,15,0.1")]
    private float _inSpeed = 6f;

    [Export(PropertyHint.Range, "0.2,15,0.1")]
    private float _outSpeed = 10f;

    [ExportCategory("Setup")]
    [Export] private PM_SurfaceState _groundSurfaceState;
    [Export] private PS_Grounded _groundState;

    private Vector3 _downPosition;
    private Vector3 _targetPosition;
    private float _currentSpeed;

    public override void _Ready()
    {
        _downPosition = new(0, _heightDisplacement, 0);

        _groundSurfaceState.Sprint.OnStart += OnStartSprint;
        _groundSurfaceState.Sprint.OnStop += OnStopSprint;
    }
    public override void _Process(double delta)
    {
        Position = Position.Lerp(_targetPosition, _currentSpeed * (float)delta);
    }

    public void OnStartSprint(object sender, EventArgs e)
    {
        _targetPosition = _downPosition;
        _currentSpeed = _inSpeed;
    }

    public void OnStopSprint(object sender, EventArgs e)
    {
        _targetPosition = Vector3.Zero;
        _currentSpeed = _outSpeed;
    }
}