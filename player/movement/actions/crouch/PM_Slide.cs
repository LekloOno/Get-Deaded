using System;
using Godot;

[GlobalClass]
public partial class PM_Slide : PM_Action
{
    [Export] public PM_Controller Controller {get; private set;}
    [Export] public PI_Slide SlideInput {get; private set;}
    [Export] public PB_Scale BodyScalor {get; private set;}
    [Export(PropertyHint.Range, "0.0, 20.0")] public float ScaleSpeed = 10f;
    [Export(PropertyHint.Range, "0.0, 20.0")] public float ResetScaleSpeed = 10f;
    [Export(PropertyHint.Range, "0.2,  1.0")] public float TargetScaleRatio {get; private set;} = 0.6f;
    [Export(PropertyHint.Range, "0.0,  1.0")] public float ForceDelay {get; private set;} = 0.12f;
    [Export(PropertyHint.Range, "0.0, 15.0")] public float Force {get; private set;} = 3f;
    [Export(PropertyHint.Range, "0.0, 10.0")] public float SlideDecayRecover {get; private set;} = 3f;
    [Export(PropertyHint.Range, "0.0, 10.0")] public float SlideDecayMinRecover {get; private set;} = 1f;
    [Export(PropertyHint.Range, "0.0,  1.0")] public float SlideDecayStrength {get; private set;} = 0.8f;
    private ulong _lastForceTime = 0;

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
        {
            ulong currentTime = Time.GetTicksMsec();
            
            // Compute decay
            float elapsedTime = (currentTime/1000f) - (_lastForceTime/1000f);
            float decayCoefficient = Mathf.Pow(Mathf.Min(SlideDecayRecover, elapsedTime)/SlideDecayRecover, SlideDecayStrength);

            Controller.AdditionalForces.AddImpulse(Controller.Velocity.Normalized() * Force * decayCoefficient);

            _lastForceTime = currentTime;
        }
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
        ulong currentTime = Time.GetTicksMsec();
        float elapsedTime = (currentTime/1000f) - (_lastForceTime/1000f);

        if (SlideDecayRecover-elapsedTime < SlideDecayMinRecover)
        {
            ulong acknwoledgedTime = (ulong)((SlideDecayRecover - SlideDecayMinRecover) * 1000f);
            _lastForceTime = currentTime - acknwoledgedTime;
        }

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