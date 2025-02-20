using System;
using Godot;

[GlobalClass]
public partial class PC_Bobbing : Node3D
{
    [Export] private PI_Sprint _sprintInput;
    [Export] private PS_Grounded _groundState;
    [Export] private PM_Controller _controller;
    [Export(PropertyHint.Range, "0.0,0.01")] private float _sprintAmplitude = 0.0007f;
    [Export(PropertyHint.Range, "0.0, 1.0")] private float _sprintWaveLength = 0.15f;
    [Export(PropertyHint.Range, "0.0,0.01")] private float _walkAmplitude = 0.0004f;
    [Export(PropertyHint.Range, "0.0, 1.0")] private float _walkWaveLength = 0.25f;

    private float _toggleSpeed = 3.0f;
    private bool _enable = true;
    private float _amplitude;
    private float _waveLength;

    public override void _Ready()
    {
        _amplitude = _walkAmplitude;
        _waveLength = _walkWaveLength;

        _sprintInput.OnStartSprinting += OnStartSprint;
        _sprintInput.OnStopSprinting += OnStopSprint;
    }
    public override void _Process(double delta)
    {
        if (!_enable) ResetPosition(delta);
        else {
            CheckMotion();
            ResetPosition(delta);
        }
    }

    private Vector3 FootStepMotion()
    {
        Vector3 pos = Vector3.Zero;
        float time = Time.GetTicksMsec()/1000f;
        pos.Y += Mathf.Sin(time/_waveLength * Mathf.Pi) * _amplitude;
        pos.X += Mathf.Sin(time/_waveLength * Mathf.Pi/2) * _amplitude * 2;
        return pos;
    }

    private void CheckMotion()
    {
        if (PHX_Vector3Ext.Flat(_controller.RealVelocity).Length() < _toggleSpeed) return;
        if (!_groundState.IsGrounded()) return;

        PlayMotion(FootStepMotion());
    }

    private void PlayMotion(Vector3 motion) => Position += motion;

    private void ResetPosition(double deltaTime)
    {
        if (Position == Vector3.Zero) return;
        Position = Position.Lerp(Vector3.Zero, (float)deltaTime);
    }

    public void OnStartSprint(object sender, EventArgs e)
    {
        _amplitude = _sprintAmplitude;
        _waveLength = _sprintWaveLength;
        //_enable = true;
    }

    public void OnStopSprint(object sender, EventArgs e)
    {
        _amplitude = _walkAmplitude;
        _waveLength = _walkWaveLength;
        //_enable = false;
    }
}