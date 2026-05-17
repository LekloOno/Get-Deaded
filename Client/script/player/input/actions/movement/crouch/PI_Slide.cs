using System;
using Godot;

[GlobalClass]
public partial class PI_Slide : Node, PI_CrouchDerived
{
    [ExportCategory("Settings")]
    [Export] private float _slideMinSpeed;
    [Export] private float _holdSlideMinSpeed;

    [ExportCategory("Setup")]
    [Export] private PM_Controller _controller;
    [Export] private PS_Grounded _groundState;
    [Export] private PI_Crouch _crouchInput;
    [Export] private PI_Sprint _sprintInput;
    public PI_Sprint SprintInput => _sprintInput;

    public EventHandler OnStartInput {get; set;}   // Called when slide is initiated
    public EventHandler OnStopInput {get; set;}    // Called when slide is cancelled through inputs
    public EventHandler OnSlowSlide;    // Called when slide is cancelled because of the speed being too low - it means the player is now crouched
    private EventHandler OnPhysics;

    public bool IsActive {get; set;} = false;

    public void InputStart()
    {
        if (StartFastEnough() || !_groundState.IsGrounded())
            StartSlide();               // Consume - start
        else
            _crouchInput.InputStart();  // Propagate to crouch
    }

    public void InputStop()
    {
        if (IsActive)
            StopSlide();                // Consume - stop
        else
            _crouchInput.InputStop();   // Propagate to crouch
    }

    private bool StartFastEnough() => _controller.RealVelocity.Length() >= _slideMinSpeed;
    private bool HoldFastEnough() => _controller.RealVelocity.Length() >= _holdSlideMinSpeed;

    public override void _PhysicsProcess(double delta) => OnPhysics?.Invoke(this, EventArgs.Empty);

    private void CheckSpeed(object sender, EventArgs e)
    {
        // Virtually propagate to crouch if sliding and not fast enough
        if (_groundState.IsGrounded() && !HoldFastEnough())
        {
            // Notify crouch to start
            IsActive = false;
            OnSlowSlide?.Invoke(this, EventArgs.Empty);
            _sprintInput.Reset();

            OnPhysics -= CheckSpeed;

            _crouchInput.StartCrouching();
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