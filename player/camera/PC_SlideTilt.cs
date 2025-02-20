using System;
using Godot;

[GlobalClass]
public partial class PC_SlideTilt : Node3D
{
    [ExportCategory("Settings")]
    [Export(PropertyHint.Range, "0,0.1,0.001")]
    private float _strength = 0.01f;
    
    [Export(PropertyHint.Range, "0,20,1")]
    private float _tiltSpeed = 5f;

    [Export(PropertyHint.Range, "0,20,1")]
    private float _resetSpeed = 5f;

    [ExportCategory("Setup")]
    [Export] private PC_Control _cameraControl;
    [Export] private PM_Controller _controller;
    [Export] private PM_SurfaceState _surfaceGroundState;

    private bool _active = false;

    public override void _Ready()
    {
        _surfaceGroundState.Slide.OnStart += OnStartSlide;
        _surfaceGroundState.Slide.OnStop += OnStopSlide;
    }
    public void OnStartSlide(object sender, EventArgs e) => _active = true;
    public void OnStopSlide(object sender, EventArgs e) => _active = false;

    public override void _Process(double delta)
    {
        if(_active)
            SlideTilt(delta);
        else
            ResetTilt(delta);
    }

    public void SlideTilt(double delta)
    {
        Vector3 flatDirAxis = _cameraControl.Basis.X;
        float velDirScalar = _controller.RealVelocity.Dot(flatDirAxis);

        float angle = (_strength * velDirScalar - Rotation.Z) * (float)delta * _resetSpeed;
        RotateZ(angle);
    }

    private void ResetTilt(double delta)
    {
        float angle = (-Rotation.Z)* (float)delta * _resetSpeed;
        RotateZ(angle); 
    }

}