using Godot;

public abstract partial class PW_ADS<T> : Resource where T : class
{
    [Export] public bool Hold {get; private set;}
    [Export] public float ScopeInTime {get; private set;}
    [Export] public float ScopeOutTime {get; private set;}
    [Export] public float FovMultiplier {get; private set;}
    [Export] public float MoveSpeedMultiplier {get; private set;} = 1f;
    private bool _active = false;
    private Camera3D _camera;

    public void Initialize(Camera3D camera)
    {
        _camera = camera;
    }

    protected abstract T GetModifiers();
    protected abstract T GetInitValue();

    public T ActivatedPress()
    {
        if (Hold)
        {
            _active = true;
            return GetModifiers();
        }
        
        if (_active)
        {
            _active = false;
            return GetInitValue();
        }

        _active = true;
        return GetModifiers();
    }

    public T ActivatedRelease()
    {
        if (Hold)
        {
            _active = false;
            return GetInitValue();
        }

        return null;
    }
}