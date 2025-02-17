using Godot;

public partial class PM_VelocityCache : Resource
{
    [Export] public ulong CacheFrameMsec {get; private set;} = 200;
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
        return Time.GetTicksMsec() - _cachedTime < CacheFrameMsec;
    }
}