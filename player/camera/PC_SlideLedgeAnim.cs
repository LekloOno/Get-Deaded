using System;
using Godot;

[GlobalClass]
public partial class PC_SlideLedgeAnim : Node3D
{
    [ExportCategory("Settings")]
    [Export] private Curve _curve;
    [Export] private float _slideStrengthX;
    [Export] private float _slideStrengthZ;
    [Export] private float _ledgeStrengthX;
    [Export] private float _ledgeStrengthZ;
    [Export] private float _duration;

    [ExportCategory("Setup")]
    [Export] private PS_Grounded _groundState;
    [Export] private PC_Control _cameraControl;
    [Export] private PM_Controller _controller;
    [Export] private PM_SurfaceState _groundSurfaceState;
    [Export] private PM_LedgeClimb _ledgeClimb;

    static private Random rng = new Random();
    private float _xDirection;
    private float _zDirection;
    private float _strengthX;
    private float _strengthZ;

    private float _curveX = 0f;

    public override void _Ready()
    {
        _groundSurfaceState.Slide.OnStart += OnStartSlide;
        _ledgeClimb.OnStart += OnStartLedge;
        SetProcess(false);
    }

    private void OnStartSlide(object sender, EventArgs e)
    {
        if (_groundState.IsGrounded())
        {
            _xDirection = 1f;
            
            Vector3 flatDirAxis = _cameraControl.Basis.X.Normalized();
            Vector3 flatVel = PHX_Vector3Ext.Flat(_controller.RealVelocity.Normalized());
            
            float dot = flatVel.Dot(flatDirAxis);
            _zDirection = (1-Mathf.Abs(dot)) * Mathf.Sign(dot);

            GD.Print(_zDirection);

            _strengthX = _slideStrengthX;
            _strengthZ = _slideStrengthZ;

            StartAnim();
        }
    }

    private void OnStartLedge(object sender, EventArgs e)
    {
        _xDirection = -1f;
        _zDirection = (rng.Next(2) * 2f) - 1f;
        
        _strengthX = _ledgeStrengthX;
        _strengthZ = _ledgeStrengthZ;

        StartAnim();
    }

    private void StartAnim()
    {
        _curveX = 0f;
        SetProcess(true);
    }

    private bool Completed() => _curveX > 1;

    public override void _Process(double delta)
    {
        if (Completed())
        {
            SetProcess(false);
            RotationDegrees = Vector3.Zero;
            return;
        }

        float curvY = _curve.Sample(_curveX); 
        float rotX = curvY * _strengthX * _xDirection;
        float rotZ = curvY * _strengthZ * _zDirection;

        RotationDegrees = new(rotX, 0, rotZ);

        _curveX += (float)delta/_duration;
    }
}