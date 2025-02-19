using Godot;

[GlobalClass]
public partial class PM_WallJump : PM_Action
{
    
    [Export] public PI_Jump JumpInput {get; private set;}
    [Export] public PM_Controller Controller {get; private set;}
    [Export] public RayCast3D WallCast {get; private set;}
    [Export(PropertyHint.Range, "0.0, 10.0")] public float Strength {get; private set;} = 5f;
    [Export(PropertyHint.Range, "0.0, 10.0")] public float Boost {get; private set;} = 1f;
    [Export(PropertyHint.Range, "0.0, 10.0")] public float MinSpeedInWall {get; private set;} = 5f;
    [Export(PropertyHint.Range, "0.0,  1.0")] public float MinBounceRatio {get; private set;} = 0.5f;
    


    public Vector3 WallJump(Vector3 velocity)
    {
        if (WallCast.IsColliding() && IsWall(WallCast.GetCollisionNormal()) && JumpInput.UseBuffer())
            return DoWallJump(velocity, WallCast.GetCollisionNormal());

        return velocity;
    }

    private Vector3 DoWallJump(Vector3 velocity, Vector3 normal)
    {
        Vector3 flatVel = new Vector3(velocity.X, 0, velocity.Z);
        
        if (flatVel.Dot(-normal) < MinSpeedInWall)
            return velocity;
            
        flatVel = flatVel.Bounce(normal);

        // Angle ratio -
        // The more the wall jump is performed against the wall, the more speed you lose
        // The more --------------is performed sideways, the less-----
        float angleRatio = flatVel.Normalized().Dot(normal.Normalized());
        angleRatio = 1 + angleRatio * MinBounceRatio - angleRatio;
        
        flatVel *= angleRatio;

        flatVel.Y = Strength;
        return flatVel;
    }

    public bool IsWall(Vector3 normal) => normal.AngleTo(Controller.UpDirection) > Controller.FloorMaxAngle;
}