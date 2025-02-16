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

    private PM_SurfaceState _currentSurface;
    public override void _Ready()
    {
        _currentSurface = CharacterBody.IsOnFloor() ? Ground : Air;

        GroundState.OnLanding += SetGroundState;
        GroundState.OnLeaving += SetAirState;
    }

    public override void _PhysicsProcess(double delta)
    {
        CharacterBody.Velocity += Accelerate(CharacterBody.Velocity, (float)delta);
        CharacterBody.Velocity = ApplyDrag(CharacterBody.Velocity, delta);
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
        // Linear Drag:
        float dragFactor = _currentSurface.CurrentData.Drag;

        dragFactor = 1f/(1f+.1f*dragFactor);    // Transform the drag

        return velocity * dragFactor;
    }

    private void SetAirState(object sender, EventArgs e)
    {
        _currentSurface = Air;
    }

    private void SetGroundState(object sender, LandingEventArgs e)
    {
        _currentSurface = Ground;
    }


}