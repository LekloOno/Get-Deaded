using System;
using Godot;

[GlobalClass]
public partial class PI_Slide : Node, PI_CrouchDerived
{
    [Export] public PM_Controller Controller {get; private set;}
    [Export] public PS_Grounded GroundState {get; private set;}
    [Export] public PI_Crouch CrouchInput {get; private set;}
    [Export] public PI_Sprint SprintInput {get; private set;}

    [Export] public bool Hold {get; private set;}
    [Export] public float SlideMinSpeed {get; private set;}
    [Export] public float HoldSlideMinSpeed {get; private set;}

    public EventHandler OnStartInput {get; set;}   // Called when slide is initiated
    public EventHandler OnStopInput {get; set;}    // Called when slide is cancelled through inputs
    public EventHandler OnSlowSlide;    // Called when slide is cancelled because of the speed being too low - it means the player is now crouched
    private EventHandler OnPhysics;

    public bool IsActive {get; set;} = false;

    public void KeyDown()
    {
        // Can consume if
        //      Can start slide - fast enough or mid air
        //      Is on non hold mode and can stop slide
        // Otherwise, propagate to crouch


        if (!IsActive && (StartFastEnough() || !GroundState.IsGrounded()))
        {
            // Consume - start
            StartSlide();
        } else if (IsActive && !Hold)
        {
            // Consume - stop
            StopSlide();
        } else
        {
            CrouchInput.KeyDown();
        }
    }

    private bool StartFastEnough() => Controller.RealVelocity.Length() >= SlideMinSpeed;
    private bool HoldFastEnough() => Controller.RealVelocity.Length() >= HoldSlideMinSpeed;

    public void KeyUp()
    {
        // Can consume if
        //      On Hold mode and can stop slide
        // Otherwise, propagate to crouch
        if (IsActive && Hold)
        {
            // Consume - stop
            StopSlide();
        } else 
        {
            CrouchInput.KeyUp();
        }
    }

    public override void _PhysicsProcess(double delta) => OnPhysics?.Invoke(this, EventArgs.Empty);

    private void CheckSpeed(object sender, EventArgs e)
    {
        // Virtually propagate to crouch if sliding and not fast enough
        if (GroundState.IsGrounded() && !HoldFastEnough())
        {
            // Notify crouch to start
            IsActive = false;
            OnSlowSlide?.Invoke(this, EventArgs.Empty);
            SprintInput.Reset();

            OnPhysics -= CheckSpeed;

            CrouchInput.StartCrouching();
        }
    }

    public void StopSlide()
    {
        ((PI_CrouchDerived)this).StopAction();
        OnPhysics -= CheckSpeed;
    }

    private void StartSlide()
    {
        ((PI_CrouchDerived)this).StartAction();
        OnPhysics += CheckSpeed;
    }
}