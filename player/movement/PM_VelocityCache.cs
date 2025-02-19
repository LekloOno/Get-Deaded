using Godot;

public partial class PM_VelocityCache : Resource
{
    [Export] public ulong CacheFrameMsec {get; private set;} = 200;
    private Vector3 _cachedVelocity = Vector3.Zero;
    private ulong _cachedTime = 0;
    private bool _inWall = false;

    public Vector3 UseCache()
    {
        Vector3 outputVel = IsCached() ? _cachedVelocity : Vector3.Zero;
        _cachedVelocity = Vector3.Zero;
        _cachedTime = 0;
        
        return outputVel;
    }

    private Vector3 UseCacheOr(Vector3 velocity)
    {
        Vector3 outputVel = TestVelocity(velocity);
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
        return Time.GetTicksMsec() - _cachedTime < CacheFrameMsec;
    }

    public Vector3 TestVelocity(Vector3 velocity)
    {
        if (IsCached())
        {
            Vector3 output = _cachedVelocity;
            output.Y = velocity.Y;
            return output;
        }
        return velocity;
    }

    public Vector3 GetVelocity(PM_Controller controller, Vector3 velocity, bool grounded, double delta)
    {
        Transform3D currentTransform = controller.GlobalTransform;
        KinematicCollision3D collision = new();

        Vector3 testVelocity = TestVelocity(velocity);

        bool wasInWall = _inWall;
        bool collideNext = controller.TestMove(
                                currentTransform,
                                testVelocity * (float)delta,
                                collision,
                                controller.SafeMargin,
                                true);

        if (collideNext)
        {
            _inWall = collision.GetAngle(0, controller.UpDirection) > controller.FloorMaxAngle;
            if(_inWall)
            {
                if (!wasInWall) Cache(velocity);
                return grounded ? controller.Velocity : controller.RealVelocity;
            }
        }

        _inWall = false;

        if (wasInWall)
            return UseCacheOr(velocity);

        return velocity;
    }
}