using System;
using Godot;

[GlobalClass]
public partial class PM_Crouch : PM_Action
{
    [Export] private PI_Crouch _crouchInput;
    [Export] private PHX_BodyScale _bodyScalor;
    
    [Export(PropertyHint.Range, "0.0, 10.0")] private float _scaleSpeed = 10f;
    [Export(PropertyHint.Range, "0.0, 10.0")] private float _resetScaleSpeed = 10f;
    [Export(PropertyHint.Range, "0.2,1.0")] private float _targetScaleRatio = 0.6f;

    public override void _Ready()
    {
        _crouchInput.OnStartInput += (o, e) => StartCrouch();
        _crouchInput.OnStopInput += (o, e) => StopCrouch();
    }

    public void StartCrouch()
    {
        _bodyScalor.SetTargetScale(_targetScaleRatio, _scaleSpeed);
        OnStart?.Invoke(this, EventArgs.Empty);
    }

    public void StopCrouch()
    {
        _bodyScalor.ResetScale(_resetScaleSpeed);
        OnStop?.Invoke(this, EventArgs.Empty);
    }
}

// OnStartCrouch
//      grounded
//          no speed - start crouch
//          speed - start slide
//      airborne
//          start slide with no boost
//          _canDash - start dash

// OnStopCrouch
//      grounded
//          start normal
//      airborne
//          stop slide

// OnLanding
//      dashing - stop dashing
//
// SpeedTooLow and grounded
//      stop sliding