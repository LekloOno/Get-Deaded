using Godot;

[GlobalClass]
public partial class PW_ADS : Resource
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

    public void Disable()
    {
        if (_active)
            Deactivate();
    }

    /// <summary>
    /// Handles pressed input.
    /// </summary>
    /// <returns>true if the following ADS state is active, false otherwise.</returns>
    public bool Pressed()
    {
        if (Hold)
            return Activate();
        
        if (_active)
            return Deactivate();

        return Activate();
    }

    /// <summary>
    /// Handle released input.
    /// </summary>
    /// <returns>true if the state of the ADS has changed, false otherwise.</returns>
    public bool Released()
    {
        if (Hold)
            return !Deactivate();

        return false;
    }

    private bool Activate()
    {
        _active = true;
        _camera.ModifyFov(FovMultiplier, ScopeInTime);
        return true;
    }

    private bool Deactivate()
    {
        _active = false;
        _camera.ResetFov(ScopeOutTime);
        return false;
    }
}