using Godot;

[GlobalClass]
public partial class PM_VelocityCache : Resource
{
    [Export] private ulong _cacheFrameMsec = 200;
    private Vector3 _cachedVelocity = Vector3.Zero;
    private ulong _cachedTime = 0;
    private bool _inWall = false;

    public Vector3 UseCache()
    {
        Vector3 outputVel = IsCached() ? _cachedVelocity : Vector3.Zero;
        DiscardCache();
        
        return outputVel;
    }

    public Vector3 UseCacheOr(Vector3 velocity)
    {
        Vector3 outputVel = TestVelocity(velocity);
        DiscardCache();
        
        return outputVel;
    }

    public void Cache(Vector3 velocity)
    {
        _cachedVelocity = velocity;
        _cachedTime = Time.GetTicksMsec();
    }

    public bool IsCached() => Time.GetTicksMsec() - _cachedTime < _cacheFrameMsec;

    public void DiscardCache()
    {
        _cachedVelocity = Vector3.Zero;
        _cachedTime = 0;
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

    public Vector3 GetVelocity(PM_Controller controller, Vector3 velocity, Vector3 WishDir, bool grounded, double delta)
    {
        Transform3D currentTransform = controller.GlobalTransform;
        KinematicCollision3D collision = new();

        Vector3 testVelocity = TestVelocity(velocity) + WishDir * 2f;

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
                
                if(collision.GetNormal().Dot(WishDir) > 0) // If inputs outward the wall, discard
                    DiscardCache();
                else if (!wasInWall)
                    Cache(velocity);
                // if (!wasInWall) Cache(velocity); It seems like doing if instead of else if results in fun behavior .. to further test out

                return grounded ? controller.Velocity : controller.RealVelocity;
            }
        }

        _inWall = false;

        if (wasInWall)
            return UseCacheOr(velocity);

        return grounded ? controller.Velocity : controller.RealVelocity;
    }
}