using System;
using Godot;

[GlobalClass]
public partial class PM_Crouch : PM_Action
{
    [Export] public PI_Crouch CrouchInput {get; private set;}
    [Export] public PB_Scale BodyScalor {get; private set;}
    [Export] public float SlideMinSpeed;
    [Export] public float ScaleSpeed;
    [Export(PropertyHint.Range, "0.2,1.0")] public float TargetScaleRatio {get; private set;}
    //[Export] public float 
    private bool _canDash;

    public EventHandler OnStartCrouch;
    public EventHandler OnStopCrouch;

    private EventHandler OnCrouchUpdate;

    public override void _Ready()
    {
        CrouchInput.OnStartInput += (o, e) => StartCrouch();
        CrouchInput.OnStopInput += (o, e) => StopCrouch();
    }

    public void CrouchUpdate()
    {
        OnCrouchUpdate?.Invoke(this, EventArgs.Empty);
        // OnStartCrouch
        //      grounded
        //          no speed - start crouch
    //              speed - start slide
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
    }

    public void StartCrouch()
    {
        BodyScalor.SetTargetScale(TargetScaleRatio, ScaleSpeed);
        OnStartCrouch?.Invoke(this, EventArgs.Empty);
    }

    public void StopCrouch()
    {
        BodyScalor.ResetScale(ScaleSpeed);
        OnStopCrouch?.Invoke(this, EventArgs.Empty);
    }
}