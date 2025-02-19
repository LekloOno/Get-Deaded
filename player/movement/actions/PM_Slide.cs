using System;
using Godot;

[GlobalClass]
public partial class PM_Slide : PM_Action
{
    [Export] public PM_Controller Controller {get; private set;}
    [Export] public PI_Slide SlideInput {get; private set;}
    [Export] public PB_Scale BodyScalor {get; private set;}
    [Export(PropertyHint.Range, "0.0, 10.0")] public float ScaleSpeed;
    [Export(PropertyHint.Range, "0.0, 10.0")] public float ResetScaleSpeed;
    [Export(PropertyHint.Range, "0.2,  1.0")] public float TargetScaleRatio {get; private set;}
    [Export(PropertyHint.Range, "0.0,  1.0")] public float ForceDelay {get; private set;} 
    [Export(PropertyHint.Range, "0.0, 15.0")] public float Force {get; private set;}

    public EventHandler OnStart;
    public EventHandler OnStop;
    public EventHandler OnSlowStop;

    private SceneTreeTimer _delayedForceTimer;


    public override void _Ready()
    {
        SlideInput.OnStartInput += (o, e) => StartSlide();
        SlideInput.OnStopInput += (o, e) => StopSlide();
        SlideInput.OnSlowSlide += (o, e) => SlowStop();
    }

    public void AddForce()
    {
        if (GroundState.IsGrounded())
            Controller.AdditionalForces.AddImpulse(Controller.Velocity.Normalized() * Force);
    }

    public void StartSlide()
    {
        if (GroundState.IsGrounded())
        {
            _delayedForceTimer = GetTree().CreateTimer(ForceDelay);
            _delayedForceTimer.Timeout += AddForce;
        }

        Controller.FloorConstantSpeed = false;
        BodyScalor.SetTargetScale(TargetScaleRatio, ScaleSpeed);
        OnStart?.Invoke(this, EventArgs.Empty);
    }

    public void StopSlide()
    {
        if(_delayedForceTimer != null)
        {
            _delayedForceTimer.Timeout -= AddForce;
            _delayedForceTimer = null;
        }

        Controller.FloorConstantSpeed = true;
        BodyScalor.ResetScale(ResetScaleSpeed);
        OnStop?.Invoke(this, EventArgs.Empty);
    }

    public void SlowStop()
    {
        Controller.FloorConstantSpeed = true;
        OnSlowStop?.Invoke(this, EventArgs.Empty);
    }
}