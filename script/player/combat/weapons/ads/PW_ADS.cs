using System;
using Godot;

[GlobalClass, Icon("res://gd_icons/weapon_system/sight_icon.svg")]
public partial class PW_ADS : WeaponSystem
{
    [Export] private bool _hold = true;
    [Export] private float _scopeInTime;
    [Export] private float _scopeOutTime;
    [Export] private float _fovMultiplier = 1f;
    [Export] private float _moveSpeedMultiplier = 0f; // Additive percent modifier
    private PM_SurfaceControl _surfaceControl;
    private PC_DirectCamera _camera;
    
    public bool Active => _active;
    public Action Started;
    public Action Stopped;
    private bool _active = false;

    /// -------------------------
    ///    ___                
    ///   | _ ) __ _  ___ ___ 
    ///   | _ \/ _` |(_-</ -_)
    ///   |___/\__,_|/__/\___|
    /// 
    /// -------------------------
    /// Base Definitions for ADS.

    #region base
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
        if (_hold)
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
        if (_hold)
            return TryDeactivate();

        return false;
    }

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
    #endregion
    
    /// ----------------------------------------------------------------
    ///  ___                 _        _  _            _    _            
    /// / __| _ __  ___  __ (_) __ _ | |(_) ___ __ _ | |_ (_) ___  _ _  
    /// \__ \| '_ \/ -_)/ _|| |/ _` || || ||_ // _` ||  _|| |/ _ \| ' \ 
    /// |___/| .__/\___|\__||_|\__,_||_||_|/__|\__,_| \__||_|\___/|_||_|
    ///      |_|                                                        
    /// ----------------------------------------------------------------
    /// Signatures for specialized ADS.
    protected virtual void SpecActivate(){}
    protected virtual void SpecDeactivate(){}
}