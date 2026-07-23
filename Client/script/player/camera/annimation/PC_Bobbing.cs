using System;
using Godot;

[GlobalClass]
public partial class PC_Bobbing : Node3D
{
    [ExportCategory("Settings")]
    [Export(PropertyHint.Range, "0,0.5,0.0001")]
    private float _sprintAmplitude = 0.002f;

    [Export(PropertyHint.Range, "0,0.3,0.01")]
    private float _sprintWaveLength = 0.15f;
    
    [Export(PropertyHint.Range, "0,0.5,0.0001")]
    private float _walkAmplitude = 0.0006f;
    
    [Export(PropertyHint.Range, "0,0.3,0.01")]
    private float _walkWaveLength = 0.16f;

    [Export]
    private float _introRate = 8f;
    [Export(PropertyHint.Range, "0,20,0.1")]
    private float _outroRate = 6.0f;
    [Export(PropertyHint.Range, "0,6,0.1")]
    private float _lateralRatio = 3.0f;
    [Export(PropertyHint.Range, "0,20,0.1")]
    private float _transitionSmoothingRate = 6.0f;
    
    [ExportCategory("Setup")]
    [Export] private PM_SurfaceState _groundSurfaceState;
    [Export] private PS_Grounded _groundState;
    [Export] private PM_Controller _controller;

    private float _toggleSpeed = 3.0f;
    private bool _active = true;

    private float _targetAmplitude;
    private float _targetWaveLength;
    private float _amplitude;
    private float _waveLength;
    
    private float _introFactor;
    private float _phaseY = 0f;
    private float _phaseX = 0f;

    public override void _Ready()
    {
        _targetAmplitude = _walkAmplitude;
        _targetWaveLength = _walkWaveLength;
        _amplitude = _walkAmplitude;
        _waveLength = _walkWaveLength;

        _groundSurfaceState.Sprint.OnStart += OnStartSprint;
        _groundSurfaceState.Sprint.OnStop += OnStopSprint;

        _groundSurfaceState.Normal.OnStart += OnStartWalk;
        _groundSurfaceState.Normal.OnStop += OnStopWalk;
    }

    public override void _ExitTree()
    {
        Position = Vector3.Zero;
    }

    public override void _Process(double delta)
    {
        float dt = (float)delta;

        float paramT = 1f - Mathf.Exp(-_transitionSmoothingRate * dt);
        _amplitude = Mathf.Lerp(_amplitude, _targetAmplitude, paramT);
        _waveLength = Mathf.Lerp(_waveLength, _targetWaveLength, paramT);
        
        bool isBobbing = !TooSlow() && _active && _groundState.IsGrounded();

        float target = isBobbing ? 1f : 0f;
        float rate = isBobbing ? _introRate : _outroRate;
        _introFactor += (target - _introFactor) * (1f - Mathf.Exp(-rate * (float)delta));

        if (_introFactor > 0.0005f || isBobbing)
            Position = FootStepMotion(dt) * _introFactor;
        else
        {
            _introFactor = 0f;
            Position = Vector3.Zero;
        }
    }

    private Vector3 FootStepMotion(float dt)
    {
        Vector3 pos = Vector3.Zero;

        float safeWaveLength = Mathf.Max(_waveLength, 0.0001f);

        _phaseY += (Mathf.Pi / safeWaveLength) * dt;
        _phaseX += (Mathf.Pi / 2f / safeWaveLength) * dt;

        _phaseY = Mathf.Wrap(_phaseY, 0f, Mathf.Tau);
        _phaseX = Mathf.Wrap(_phaseX, 0f, Mathf.Tau);

        pos.Y += Mathf.Sin(_phaseY) * _amplitude;
        pos.X += Mathf.Sin(_phaseX) * _amplitude * _lateralRatio;

        return pos;
    }

    public void OnStartSprint()
    {
        _targetAmplitude = _sprintAmplitude;
        _targetWaveLength = _sprintWaveLength;
        _active = true;
    }
    public void OnStartWalk()
    {
        _targetAmplitude = _walkAmplitude;
        _targetWaveLength = _walkWaveLength;
        _active = true;
    }

    public void OnStopSprint() => _active = false;
    public void OnStopWalk() => _active = false;

    private bool TooSlow() => MATH_Vector3Ext.Flat(_controller.RealVelocity).Length() < _toggleSpeed;

}