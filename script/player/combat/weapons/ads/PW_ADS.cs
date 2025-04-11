using Godot;

[GlobalClass]
public partial class PW_ADS : Node3D
{
    [Export] public bool Hold {get; private set;}
    [Export] private float _scopeInTime;
    [Export] private float _scopeOutTime;
    [Export] private float _fovMultiplier = 1f;
    [Export] private float _moveSpeedMultiplier = 0f; // Additive percent modifier
    private bool _active = false;
    private PM_SurfaceControl _surfaceControl;
    private PC_DirectCamera _camera;

    public void Initialize(PC_DirectCamera camera, PM_SurfaceControl surfaceControl)
    {
        _camera = camera;
        _surfaceControl = surfaceControl;
    }

    /// <summary>
    /// Disables the ADS.
    /// </summary>
    /// <returns>true if it was active, false otherwise.</returns>
    public bool Disable()
    {
        bool wasActive = _active;

        if (wasActive)
            Deactivate();
        
        return wasActive;
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
    
    protected virtual void InnerActivate(){}
    protected virtual void InnerDeactivate(){}

    private bool Activate()
    {
        _active = true;
        _camera.ModifyFov(_fovMultiplier, _scopeInTime);
        _surfaceControl.SpeedModifiers.Add(_moveSpeedMultiplier);
        InnerActivate();
        return true;
    }

    private bool Deactivate()
    {
        _active = false;
        _camera.ResetFov(_scopeOutTime);
        _surfaceControl.SpeedModifiers.Remove(_moveSpeedMultiplier);
        InnerDeactivate();
        return false;
    }
}