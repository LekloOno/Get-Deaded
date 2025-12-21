using System;
using Godot;

[GlobalClass]
public partial class PC_Bobbing : Node3D
{
    [ExportCategory("Settings")]
    [Export(PropertyHint.Range, "0,0.004,0.0001")]
    private float _sprintAmplitude = 0.002f;

    [Export(PropertyHint.Range, "0,0.3,0.01")]
    private float _sprintWaveLength = 0.15f;
    
    [Export(PropertyHint.Range, "0,0.004,0.0001")]
    private float _walkAmplitude = 0.0006f;
    
    [Export(PropertyHint.Range, "0,0.3,0.01")]
    private float _walkWaveLength = 0.16f;

    [Export(PropertyHint.Range, "0,5,0.1")]
    private float _resetSpeed = 1.5f;
    
    [ExportCategory("Setup")]
    [Export] private PM_SurfaceState _groundSurfaceState;
    [Export] private PS_Grounded _groundState;
    [Export] private PM_Controller _controller;

    private float _toggleSpeed = 3.0f;
    private bool _active = true;
    private float _amplitude;
    private float _waveLength;

    public override void _Ready()
    {
        _amplitude = _walkAmplitude;
        _waveLength = _walkWaveLength;

        _groundSurfaceState.Sprint.OnStart += OnStartSprint;
        _groundSurfaceState.Sprint.OnStop += OnStopSprint;

        _groundSurfaceState.Normal.OnStart += OnStartWalk;
        _groundSurfaceState.Normal.OnStop += OnStopWalk;
    }
    public override void _Process(double delta)
    {
        if(TooSlow() || !_active || !_groundState.IsGrounded())
        {
            ResetPosition(delta * _resetSpeed);
            return;
        }

        Position += FootStepMotion();
        ResetPosition(delta);
    }

    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.Zero;
        float time = PHX_Time.ScaledTicksMsec/1000f;
        pos.Y += Mathf.Sin(time/_waveLength * Mathf.Pi) * _amplitude;
        pos.X += Mathf.Sin(time/_waveLength * Mathf.Pi/2) * _amplitude * 2;
        return pos;
    }

    private void ResetPosition(double deltaTime)
    {
        if (Position == Vector3.Zero) return;
        Position = Position.Lerp(Vector3.Zero, (float)deltaTime);
    }

    public void OnStartSprint(object sender, EventArgs e)
    {
        _amplitude = _sprintAmplitude;
        _waveLength = _sprintWaveLength;
        _active = true;
    }
    public void OnStartWalk(object sender, EventArgs e)
    {
        _amplitude = _walkAmplitude;
        _waveLength = _walkWaveLength;
        _active = true;
    }

    public void OnStopSprint(object sender, EventArgs e) => _active = false;
    public void OnStopWalk(object sender, EventArgs e) => _active = false;

    private bool TooSlow() => MATH_Vector3Ext.Flat(_controller.RealVelocity).Length() < _toggleSpeed;

}