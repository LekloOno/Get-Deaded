using System;
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
    [Export] public PM_SurfaceState Ground {get; private set;}
    [Export] public PM_SurfaceState Air {get; private set;}
    [Export] public ulong LandGroundDelayMsec {get; private set;} = 0;  // The delay after landing before the surface data is updated to ground.
    private SceneTreeTimer _delayedGroundTimer;

    private PM_SurfaceState _currentSurface;
    private ulong _landingTime;

    public override void _Ready()
    {
        _currentSurface = Air;

        GroundState.OnLanding += SetGroundState;
        GroundState.OnLeaving += SetAirState;
    }

    public Vector3 Accelerate(Vector3 velocity, float delta)
    {
        Vector3 direction = WalkProcess.WishDir;
        float speed = _currentSurface.CurrentData.MaxSpeed;
        float accel = _currentSurface.CurrentData.MaxAccel;
        return MovementPhysics.Acceleration(speed, accel, velocity, direction, delta);
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

        _currentSurface = Air;
    }

    private void SetGroundState(object sender, LandingEventArgs e)
    {
        _delayedGroundTimer = GetTree().CreateTimer(LandGroundDelayMsec/1000f);
        _delayedGroundTimer.Timeout += SetGroundState;
    }

    private void SetGroundState()
    {
        _currentSurface = Ground;
    }
}