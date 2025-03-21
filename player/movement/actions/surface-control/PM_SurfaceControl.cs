using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public partial class PM_SurfaceControl : PM_Action
{
    // Grounded or Airborne
    // Sprint, Crouch, Slide

    // Grounded | Airborne
    // ---------|---------
    // Sprint   | Sprint (does not affect speed)
    // Crouch   | Crouch
    // Slide    | Slide


    // Normal -- OnSprinting -> Sprint
    // Normal -- OnCrouching -> Crouch
    // Normal -- OnSliding   -> Slide   (very unlikely)

    // Sprint -- OnStopSprinting -> Normal
    // Sprint -- OnSliding       -> Slide
    // Sprint -- OnCrouching     -> Crouch  (very unlikely)

    // Slide  -- OnStopSliding   -> Normal
    // Slide  -- OnSlowSliding   -> Crouch

    // Crouch -- OnStopCrouching -> Normal


    // OnSprinting -> Sprint

    // OnSliding   -> Slide

    // OnStopSprinting -> Normal
    // OnStopSliding   -> Normal
    // OnStopCrouching -> Normal

    // OnSlowSliding   -> Crouch
    // OnCrouching     -> Crouch
    [ExportCategory("Settings")]
    [Export] private ulong LandGroundDelayMsec = 50;  // The delay after landing before the surface data is updated to ground.
    
    [ExportCategory("Setup")]
    [Export] private PI_Walk _walkInput;
    [Export] private PS_Grounded _groundState;
    [Export] private PM_SurfaceState _ground;
    [Export] private PM_SurfaceState _air;
    public PHX_AdditiveModifiers SpeedModifiers {get; private set;} = new();       // Modifiers are additive. Resulting speed is _speedModifiers.sum() * _speed
    private SceneTreeTimer _delayedGroundTimer;

    private PM_SurfaceState _currentSurface;
    private ulong _landingTime;

    public override void _Ready()
    {
        _currentSurface = _groundState.IsGrounded() ? _ground : _air;

        _groundState.OnLanding += SetGroundState;
        _groundState.OnLeaving += SetAirState;
    }

    public Vector3 Accelerate(Vector3 velocity, float delta)
    {
        Vector3 direction = _walkInput.WishDir;
        float speed = _currentSurface.CurrentData.MaxSpeed;
        float accel = _currentSurface.CurrentData.MaxAccel;
        return PHX_MovementPhysics.Acceleration(speed * SpeedModifiers.Result(), accel, velocity, direction, delta);
    }

    public Vector3 ApplyDrag(Vector3 velocity, double deltaTime)
    {
        float dragFactor = _currentSurface.CurrentData.Drag;

        dragFactor = 1f/(1f+(float)deltaTime*dragFactor);    // Transform the drag to a velocity coeficient

        return velocity * dragFactor;
    }

    private void SetAirState(object sender, EventArgs e)
    {
        if(_delayedGroundTimer != null)
        {
            _delayedGroundTimer.Timeout -= SetGroundState;
            _delayedGroundTimer = null;
        }

        _currentSurface = _air;
    }

    private void SetGroundState(object sender, LandingEventArgs e)
    {
        _delayedGroundTimer = GetTree().CreateTimer(LandGroundDelayMsec/1000f);
        _delayedGroundTimer.Timeout += SetGroundState;
    }

    private void SetGroundState()
    {
        _currentSurface = _ground;
    }
}