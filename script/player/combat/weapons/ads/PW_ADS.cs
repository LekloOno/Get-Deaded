using System;
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
    
    public Action Started;
    public Action Stopped;

    public void Initialize(PC_DirectCamera camera, PM_SurfaceControl surfaceControl)
    {
        _camera = camera;
        _surfaceControl = surfaceControl;
    }

    /// <summary>
    /// Disables the ADS.
    /// </summary>
    /// <returns>true if it was active, false otherwise.</returns>
    public bool Disable() => TryDeactivate();

    /// <summary>
    /// Handles pressed input.
    /// </summary>
    /// <returns>true if the state of the ADS has changed, false otherwise.</returns>
    public bool Press()
    {
        if (Hold)
            return TryActivate();
        

        if (_active)
            Deactivate();
        else
            Activate();
        return true;
    }

    /// <summary>
    /// Handle released input.
    /// </summary>
    /// <returns>true if the state of the ADS has changed, false otherwise.</returns>
    public bool Release()
    {
        if (Hold)
            return TryDeactivate();

        return false;
    }
    
    protected virtual void SpecActivate(){}
    protected virtual void SpecDeactivate(){}

    private bool TryActivate()
    {
        if (_active)
            return false;

        Activate();
        return true;
    }

    private void Activate()
    {
        _active = true;
        _camera.ModifyFov(_fovMultiplier, _scopeInTime);
        _surfaceControl.SpeedModifiers.Add(_moveSpeedMultiplier);
        SpecActivate();
        Started?.Invoke();
    }

    private bool TryDeactivate()
    {
        if (!_active)
            return false;
            
        Deactivate();
        return true;
    }

    private void Deactivate()
    {
        _active = false;
        _camera.ResetFov(_scopeOutTime);
        _surfaceControl.SpeedModifiers.Remove(_moveSpeedMultiplier);
        SpecDeactivate();
        Stopped?.Invoke();
    }
}