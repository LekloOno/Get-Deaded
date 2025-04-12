using System;
using Godot;
using Godot.Collections;

[GlobalClass]
public abstract partial class PW_Weapon : Node3D
{
    [Export] public float MoveSpeedModifier {get; private set;} = 0f;   // An additive modifier to set. - is a malus + is a bonus
    [Export] public float SwitchInTime {get; private set;}
    [Export] public float SwitchOutTime {get; private set;}
    [Export] public float ReloadTime {get; private set;}
    [Export] public float TacticalReloadTime {get; private set;}
    [Export] public float ReloadReadyTime {get; private set;}            // Additionnal time before the weapon is ready once it's reloaded, allow annimation cancels
    [Export] protected Array<PW_Fire> _fires;
    [Export] protected PW_ADS _ads;

    [ExportCategory("Visuals")]
    [Export] protected Node3D _barrel;
    [Export] public Texture2D Icon {get; private set;}
    [Export] public Color IconColor {get; private set;}
    
    public bool ADSActive => _ads == null ? false : _ads.Active;
    public EventHandler<ShotHitEventArgs> Hit;
    public Action Shot;
    public Action ADSStarted;
    public Action ADSStopped;
    
    protected PW_Fire _currentFire;
    protected Node3D _sight;
    private PM_SurfaceControl _surfaceControl;

    /// -----------------------------
    ///      ___                
    ///     | _ ) __ _  ___ ___ 
    ///     | _ \/ _` |(_-</ -_)
    ///     |___/\__,_|/__/\___|
    /// 
    /// -----------------------------
    /// Base Definitions for weapons.

    #region base
    /// <summary>
    /// Initialize the external references for the weapon and propagate initialization to its children components.
    /// </summary>
    /// <param name="shakeableCamera">A shakeable node for camera shakes.</param>
    /// <param name="camera">The owner's camera.</param>
    /// <param name="sight">The owner's sight.</param>
    /// <param name="surfaceControl">The owner's surface control.</param>
    /// <param name="recoilController">The owner's recoil controller.</param>
    /// <param name="owberBody">The owner's external forces manager.</param>
    public void Initialize(PC_Shakeable shakeableCamera, PC_DirectCamera camera, Node3D sight, PM_SurfaceControl surfaceControl, PC_Recoil recoilController, GB_ExternalBodyManager owberBody)
    {
        _sight = sight;
        _surfaceControl = surfaceControl;
        if (_ads != null)
        {
            _ads.Initialize(camera, surfaceControl);
            _ads.Started += StartADS;
            _ads.Stopped += StopADS;
        }
        SpecInitialize(shakeableCamera, recoilController, owberBody);
        _currentFire = InitCurrentFire();
    }

    public void Disable()
    {
        _ads?.Disable();
        SpecDisable();
    }
    
    /// <summary>
    /// Allows to retrieve existing fire modes for this weapon.
    /// </summary>
    /// <returns></returns>
    public Array<PW_Fire> GetFireModes() => _fires;

    public void ResetBuffer()
    {
        foreach (PW_Fire fire in _fires)
            fire.ResetBuffer();
    }

    public void SecondaryPress()
    {
        if (_ads == null)
            SpecSecondaryPress();
        else
            _ads.Press();
    }

    public void SecondaryRelease()
    {
        if (_ads == null)
            SpecSecondaryRelease();
        else
            _ads.Release();
    }

    /// <summary>
    /// Check if this weapon can start realoading.
    /// </summary>
    /// <param name="reloadTime">The expected reload time. Typically corresponds either to the weapon tactical or normal reload time.</param>
    /// <returns>true if the weapon can reload, false otherwise.</returns>
    public bool CanReload(out float reloadTime)
    {
        bool canReload = SpecCanReload(out bool tactical);
        reloadTime = tactical ? TacticalReloadTime : ReloadTime;
        return canReload;
    }

    private void StartADS()
    {
        SpecStartADS();
        ADSStarted?.Invoke();
    }
    private void StopADS()
    {
        SpecStopADS();
        ADSStopped?.Invoke();
    }

    #endregion

    /// ----------------------------------------------------------------
    ///  ___                 _        _  _            _    _            
    /// / __| _ __  ___  __ (_) __ _ | |(_) ___ __ _ | |_ (_) ___  _ _  
    /// \__ \| '_ \/ -_)/ _|| |/ _` || || ||_ // _` ||  _|| |/ _ \| ' \ 
    /// |___/| .__/\___|\__||_|\__,_||_||_|/__|\__,_| \__||_|\___/|_||_|
    ///      |_|                                                        
    /// ----------------------------------------------------------------
    /// Signatures for specialized weapons.
    
    #region abstract
    public abstract bool PickAmmo(int amount, bool magazine, int targetFireIndex);

    /// <summary>
    /// Allow for some specific initialization.
    /// </summary>
    protected abstract void SpecInitialize(PC_Shakeable shakeableCamera, PC_Recoil recoilController, GB_ExternalBodyManager owberBody);
    protected virtual PW_Fire InitCurrentFire() => _fires[0];
    /// <summary>
    /// Allow for some specific disabling process.
    /// </summary>
    protected virtual void SpecDisable() => _currentFire.Disable();
    

    /// <summary>
    /// Called when the primary input is pressed down. Will typically handle shooting process.
    /// </summary>
    public virtual bool PrimaryPress() => _currentFire.Press();
    /// <summary>
    /// Called when the primary input is released up. Could handle some special behaviors, or shooting.
    /// </summary>
    public virtual bool PrimaryRelease() => _currentFire.Release();
    /// <summary>
    /// Called if the ADS handler didn't consume the incoming secondary press input.
    /// </summary>
    protected abstract bool SpecSecondaryPress();
    /// <summary>
    /// Called if the ADS handler didn't consume the incoming secondary release input.
    /// </summary>
    protected abstract bool SpecSecondaryRelease();


    /// <summary>
    /// Called right after handling a starting ADS input.
    /// </summary> 
    protected abstract void SpecStartADS();
    /// <summary>
    /// Called right after handling a stopping ADS input.
    /// </summary>
    protected abstract void SpecStopADS();

    /// <summary>
    /// Specialize the reload check using the weapon's fire(s).
    /// </summary>
    /// <param name="tactical">Secondary output to indicate if a tactical or normal reload should be applied.</param>
    /// <returns>true if the weapon can reload, false otherwise.</returns>
    protected virtual bool SpecCanReload(out bool tactical)
    {
        foreach (PW_Fire fire in _fires)
            if (fire.CanReload(out tactical)) return true;
        
        tactical = false;
        return false;
    }

    public virtual void Reload()
    {
        foreach (PW_Fire fire in _fires)
            fire.Reload();
    }

    #endregion
}