using Godot;

public abstract partial class PW_ADS<T> : Resource where T : class
{
    [Export] public bool Hold {get; private set;}
    [Export] public float ScopeInTime {get; private set;}
    [Export] public float ScopeOutTime {get; private set;}
    [Export] public float FovMultiplier {get; private set;} = 1f;
    [Export] public float MoveSpeedMultiplier {get; private set;} = 1f;
    private bool _active = false;
    private PC_DirectCamera _camera;

    public void Initialize(PC_DirectCamera camera)
    {
        _camera = camera;
    }

    protected abstract T GetModifiers();
    protected abstract T GetInitValue();
    
    public void Disable()
    {
        if (_active)
            Deactivate();
    }

    public T Pressed()
    {
        if (Hold)
        {
            Activate();
            return GetModifiers();
        }
        
        if (_active)
        {
            Deactivate();
            return GetInitValue();
        }

        Activate();
        return GetModifiers();
    }

    public T Released()
    {
        if (Hold)
        {
            Deactivate();
            return GetInitValue();
        }

        return null;
    }

    private void Activate()
    {
        _active = true;
        _camera.ModifyFov(FovMultiplier, ScopeInTime);
    }

    private void Deactivate()
    {
        _active = false;
        _camera.ResetFov(ScopeOutTime);
    }
}