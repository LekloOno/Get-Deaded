using Godot;

public partial class PM_VelocityCache : Resource
{
    [Export] public ulong CacheFrameMsec {get; private set;} = 200;
    //[Export] public PM_StepClimb StepClimb {get; private set;}
    private Vector3 _cachedVelocity = Vector3.Zero;
    private ulong _cachedTime = 0;

    public Vector3 UseCache()
    {   
        Vector3 outputVel = IsCached() ? _cachedVelocity : Vector3.Zero;
        _cachedVelocity = Vector3.Zero;
        _cachedTime = 0;
        
        return outputVel;
    }

    public void Cache(Vector3 velocity)
    {
        _cachedVelocity = velocity;
        _cachedTime = Time.GetTicksMsec();
    }

    public bool IsCached()
    {
        //return false;
        return Time.GetTicksMsec() - _cachedTime < CacheFrameMsec;
    }

    public Vector3 GetVelocity(PM_Controller controller, Vector3 velocity, Vector3 pureVelocity, double delta)
    {
        Transform3D currentTransform = controller.GlobalTransform;
        KinematicCollision3D collision = new();

        if (controller.TestMove(
                currentTransform,
                pureVelocity * (float)delta,
                collision,
                controller.SafeMargin,
                true)
            )
        {
            if(collision.GetAngle(0, controller.UpDirection) > controller.FloorMaxAngle)
            {
                if (!IsCached()) Cache(pureVelocity);

                //Vector3 nextVel = (velocity.Y == 0) ? new Vector3(controller.RealVelocity.X, 0, controller.RealVelocity.Z) : controller.RealVelocity;
                //return StepClimb.DoClimb(currentTransform, nextVel, wishDir, collision, controller.GetWorld3D().DirectSpaceState, delta);
                //return nextVel;
                
                if(velocity.Y == 0)
                    return new Vector3(controller.RealVelocity.X, 0, controller.RealVelocity.Z);
                return controller.RealVelocity;
            }
        }
        if (IsCached())
            return UseCache();

        return velocity;
    }
}