using System;
using Godot;

[GlobalClass]
public partial class PM_Slide : PM_Action
{
    [Export] private PI_Slide _slideInput;
    [Export] private PM_Controller _controller;
    [Export] private PS_Grounded _groundState;
    [Export] private PHX_BodyScale _bodyScalor;
    [Export(PropertyHint.Range, "0.0, 20.0")] private float _scaleSpeed = 10f;
    [Export(PropertyHint.Range, "0.0, 20.0")] private float _resetScaleSpeed = 10f;
    [Export(PropertyHint.Range, "0.2,  1.0")] private float _targetScaleRatio = 0.6f;
    [Export(PropertyHint.Range, "0.0,  1.0")] private float _forceDelay = 0.12f;
    [Export(PropertyHint.Range, "0.0, 15.0")] private float _strength = 3f;
    [Export(PropertyHint.Range, "0.0, 10.0")] private float _slideDecayRecover = 3f;
    [Export(PropertyHint.Range, "0.0, 10.0")] private float _slideDecayMinRecover = 1f;
    [Export(PropertyHint.Range, "0.0,  1.0")] private float _slideDecayStrength = 0.8f;

    public bool IsActive {get; private set;} = false;
    private ulong _lastForceTime = 0;
    public EventHandler OnSlowStop;

    private SceneTreeTimer _delayedForceTimer;


    public override void _Ready()
    {
        _slideInput.OnStartInput += (o, e) => StartSlide();
        _slideInput.OnStopInput += (o, e) => StopSlide();
        _slideInput.OnSlowSlide += (o, e) => SlowStop();
    }

    public void AddForce()
    {
        if (_groundState.IsGrounded())
        {
            ulong currentTime = Time.GetTicksMsec();
            
            // Compute decay
            float elapsedTime = (currentTime/1000f) - (_lastForceTime/1000f);
            float decayCoefficient = Mathf.Pow(Mathf.Min(_slideDecayRecover, elapsedTime)/_slideDecayRecover, _slideDecayStrength);

            Vector3 direction = _controller.Velocity.Normalized();
            _controller.AdditionalForces.AddImpulse(direction * _strength * decayCoefficient);

            _lastForceTime = currentTime;
        }
    }

    public void StartSlide()
    {
        if (_groundState.IsGrounded())
        {
            _delayedForceTimer = GetTree().CreateTimer(_forceDelay);
            _delayedForceTimer.Timeout += AddForce;
        }

        _controller.FloorConstantSpeed = false;
        _bodyScalor.SetTargetScale(_targetScaleRatio, _scaleSpeed);

        IsActive = true;
        //_controller.FloorConstantSpeed = false;
        //_controller.FloorStopOnSlope = false;
        OnStart?.Invoke(this, EventArgs.Empty);
    }

    public void StopSlide()
    {
        ulong currentTime = Time.GetTicksMsec();
        float elapsedTime = (currentTime/1000f) - (_lastForceTime/1000f);

        if (_slideDecayRecover-elapsedTime < _slideDecayMinRecover)
        {
            ulong acknwoledgedTime = (ulong)((_slideDecayRecover - _slideDecayMinRecover) * 1000f);
            _lastForceTime = currentTime - acknwoledgedTime;
        }

        if(_delayedForceTimer != null)
        {
            _delayedForceTimer.Timeout -= AddForce;
            _delayedForceTimer = null;
        }

        //_controller.FloorConstantSpeed = true;
        _bodyScalor.ResetScale(_resetScaleSpeed);

        IsActive = false;
        //_controller.FloorConstantSpeed = true;
        //_controller.FloorStopOnSlope = true;
        OnStop?.Invoke(this, EventArgs.Empty);
    }

    public void SlowStop()
    {
        //_controller.FloorConstantSpeed = true;

        IsActive = false;
        OnSlowStop?.Invoke(this, EventArgs.Empty);
    }
}