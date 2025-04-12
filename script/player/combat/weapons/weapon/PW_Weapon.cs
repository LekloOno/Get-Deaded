using System;
using System.Collections.Generic;
using Godot;

[GlobalClass]
public abstract partial class PW_Weapon : Node3D
{
    [Export] public float MoveSpeedModifier {get; private set;} = 0f;   // An additive modifier to set. - is a malus + is a bonus
    [Export] public float SwitchInTime {get; private set;}
    [Export] public float SwitchOutTime {get; private set;}
    [Export] public float ReloadTime {get; private set;}
    [Export] public float TacticalReloadTime {get; private set;}
    [Export] public float ReloadReadyTime {get; private set;}            // Additionnal time before the weapon is ready once it's reloaded, allow annimation cancels
    [Export] protected PW_ADS _ads;

    [ExportCategory("Visuals")]
    [Export] protected Node3D _barrel;
    [Export] public Texture2D Icon {get; private set;}
    [Export] public Color IconColor {get; private set;}
    
    public bool ADSActive => _ads.Active;
    public EventHandler<ShotHitEventArgs> Hit;
    public Action Shot;
    public Action ADSStarted;
    public Action ADSStopped;
    
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
    }   

    public void Disable()
    {
        _ads?.Disable();
        SpecDisable();
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

    public bool PrimaryPress() => SpecPrimaryPress();       // For naming consistency
    public bool PrimaryRelease() => SpecPrimaryRelease();   // For naming consistency
    

    public void Reload() => SpecReload();                   // For naming consistency

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
    /// <summary>
    /// Allows to retrieve existing fire modes for this weapon.
    /// </summary>
    /// <returns></returns>
    public abstract List<PW_Fire> GetFireModes();
    public abstract void ResetBuffer();
    public abstract bool PickAmmo(int amount, bool magazine, int targetFireIndex);

    /// <summary>
    /// Allow for some specific initialization.
    /// </summary>
    protected virtual void SpecInitialize(PC_Shakeable shakeableCamera, PC_Recoil recoilController, GB_ExternalBodyManager owberBody) {}
    /// <summary>
    /// Allow for some specific disabling process.
    /// </summary>
    protected abstract void SpecDisable();

    /// <summary>
    /// Called when the primary input is pressed down. Will typically handle shooting process.
    /// </summary>
    protected abstract bool SpecPrimaryPress();
    /// <summary>
    /// Called when the primary input is released up. Could handle some special behaviors, or shooting.
    /// </summary>
    protected abstract bool SpecPrimaryRelease();
    /// <summary>
    /// Called if the ADS handler didn't consume the incoming secondary input pressed down.
    /// </summary>
    protected abstract bool SpecSecondaryPress();
    /// <summary>
    /// Called if the ADS handler didn't consume the incoming secondary input pressed up.
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
    /// Called when the reload input is pressed down.
    /// </summary>
    protected abstract void SpecReload();
    /// <summary>
    /// Specialize the reload check using the weapon's fire(s).
    /// </summary>
    /// <param name="tactical">Secondary output to indicate if a tactical or normal reload should be applied.</param>
    /// <returns>true if the weapon can reload, false otherwise.</returns>
    protected abstract bool SpecCanReload(out bool tactical);

    #endregion
}