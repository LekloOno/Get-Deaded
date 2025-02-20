using System;
using Godot;

[GlobalClass]
public partial class PM_WallJump : PM_Action
{
    
    [Export] public PI_Jump JumpInput {get; private set;}
    [Export] public PM_Controller Controller {get; private set;}
    [Export] public PM_LedgeClimb LedgeClimb {get; private set;}
    [Export] public RayCast3D WallCastLow {get; private set;}
    [Export] public RayCast3D WallCastHigh {get; private set;}
    [Export(PropertyHint.Range, "0.0, 10.0")] public float Strength {get; private set;} = 5f;
    [Export(PropertyHint.Range, "0.0, 10.0")] public float Boost {get; private set;} = 1f;
    [Export(PropertyHint.Range, "0.0, 10.0")] public float MinSpeedInWall {get; private set;} = 5f;
    [Export(PropertyHint.Range, "0.0,  1.0")] public float MinBounceRatio {get; private set;} = 0.5f;
    // The velocity coefficient when straight facing the wall. The more you're facing the wall, the less speed you will keep.

    public EventHandler OnWallJump;

    public Vector3 WallJump(Vector3 velocity)
    {
        if (!JumpInput.IsBuffered())
            return velocity;            // Nothing to do

        Vector3 normal = new();
        if (!IsCollidingWall(ref normal))
            return LedgeClimb.LedgeClimb(velocity); // Propagate to Ledge

        if (!IsWall(normal))
            return LedgeClimb.LedgeClimb(velocity); // Propagate to Ledge

        Vector3 flatVel = new Vector3(velocity.X, 0, velocity.Z);
        if (TooSlow(flatVel, normal))
            return LedgeClimb.LedgeClimb(velocity); // Propagate to Ledge

        JumpInput.UseBuffer(); 
        return DoWallJump(velocity, WallCastLow.GetCollisionNormal()); // Do it !
    }

    private Vector3 DoWallJump(Vector3 velocity, Vector3 normal)
    {
        velocity = velocity.Bounce(normal);

        // Angle ratio -
        // The more the wall jump is performed against the wall, the more speed you lose
        // The more --------------is performed sideways, the less-----
        float angleRatio = velocity.Normalized().Dot(normal.Normalized());
        angleRatio = 1 + angleRatio * MinBounceRatio - angleRatio;
        
        velocity *= angleRatio;

        velocity.Y = Strength;
        OnWallJump?.Invoke(this, EventArgs.Empty);
        return velocity;
    }

    private bool IsCollidingWall(ref Vector3 normal)
    {
        if(WallCastLow.IsColliding())
        {
            normal = WallCastLow.GetCollisionNormal();
            return true;
        }

        if(WallCastHigh.IsColliding())
        {
            GD.Print("high");
            normal = WallCastHigh.GetCollisionNormal();
            return true;
        }

        return false;
    }

    private bool TooSlow(Vector3 velocity, Vector3 normal) => velocity.Dot(-normal) < MinSpeedInWall;
    private bool IsWall(Vector3 normal) => normal.AngleTo(Controller.UpDirection) > Controller.FloorMaxAngle;
}